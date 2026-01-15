using Data;
using MessagePack;

namespace Script
{
    public class Room_SendChat : Handler<RoomService, MsgRoomSendChat, ResRoomSendChat>
    {
        public Room_SendChat(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendChat;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendChat msg, ResRoomSendChat res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} content {msg.content}");
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                return ECode.RoomNotExist;
            }

            RoomUser? user = room.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotInRoom;
            }

            // -> redis

            var message = new ChatMessage();
            message.messageId = this.service.roomMessageIdSnowflakeScript.NextRoomMessageId();
            message.roomId = room.roomId;
            message.senderId = user.userId;
            message.senderName = string.Empty;
            message.senderAvatar = string.Empty;
            message.type = msg.type;
            message.content = msg.content;
            message.timestamp = TimeUtils.GetTime();
            message.replyTo = null;

            byte[] messageBytes = MessagePackSerializer.Serialize(message);
            await this.server.roomMessagesRedis.Add(room.roomId, messageBytes);

            this.service.sd.recentMessages.Enqueue(message);
            if (this.service.sd.recentMessages.Count > 100)
            {
                this.service.sd.recentMessages.Dequeue();
            }

            // -> other users

            Dictionary<int, List<RoomUser>> dict = room.userDict
                .GroupBy(pair => pair.Value.gatewayServiceId, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var pair in dict)
            {
                int gatewayServiceId = pair.Key;
                List<RoomUser> roomUsers = pair.Value;

                List<long> userIds = roomUsers.Select(x => x.userId).ToList();
                ECode e = this.service.gatewayServiceProxy.BroadcastToClient(gatewayServiceId, userIds, MsgType.A_RoomChat, message);
                if (e == ECode.NotConnected)
                {
                    this.service.logger.Warn($"{this.msgType} gatewayServiceId {gatewayServiceId} is not connected");
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendChat msg, ECode e, ResRoomSendChat res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}