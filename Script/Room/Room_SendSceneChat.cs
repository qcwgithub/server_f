using Data;
using MessagePack;

namespace Script
{
    [AutoRegister]
    public class Room_SendSceneChat : Handler<RoomService, MsgRoomSendSceneChat, ResRoomSendSceneChat>
    {
        public Room_SendSceneChat(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendSceneChat;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendSceneChat msg, ResRoomSendSceneChat res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} type {msg.type} content {msg.content}");

            ECode e = ChatUtils.CheckChatMessageType(msg.type);
            if (e != ECode.Success)
            {
                return e;
            }

            ServerConfig.MessageConfig messageConfig = this.server.data.serverConfig.sceneMessageConfig;

            e = this.service.chatScript.CheckRoomSendSceneChat(msg, messageConfig);
            if (e != ECode.Success)
            {
                return e;
            }

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

            long now = TimeUtils.GetTime();
            if (user.lastSendChatStamp > 0 && now - user.lastSendChatStamp < messageConfig.minIntervalMs)
            {
                return ECode.Chat_TooFast;
            }

            if (user.sendChatTimestamps.Count >= messageConfig.periodMaxCount)
            {
                int count = user.sendChatTimestamps.Count(ts => now - ts < messageConfig.periodMs);
                if (count >= messageConfig.periodMaxCount)
                {
                    return ECode.Chat_TooFast;
                }
            }

            //// ok

            // last send
            user.lastSendChatStamp = now;
            user.sendChatTimestamps.RemoveAll(ts => now - ts >= messageConfig.periodMs);
            user.sendChatTimestamps.Add(now);

            // create message
            var message = new ChatMessage();
            message.messageId = ++room.sceneInfo.messageId;
            message.roomId = room.roomId;
            message.senderId = user.userId;
            message.senderName = string.Empty;
            message.senderAvatar = string.Empty;
            message.type = msg.type;
            message.content = msg.content;
            message.timestamp = TimeUtils.GetTime();
            message.replyTo = 0;
            message.senderName = msg.userName;
            message.senderAvatarIndex = msg.avatarIndex;
            message.clientMessageId = msg.clientMessageId;
            message.status = ChatMessageStatus.Normal;
            message.imageContent = msg.imageContent;

            // -> redis
            await this.server.roomMessagesRedis.Add(message);

            if (message.messageId % 100 == 0)
            {
                await this.server.roomMessagesRedis.Trim(room.roomId, messageConfig.maxMessagesCount);
            }

            // -> memory

            room.recentMessages.Enqueue(message);
            while (room.recentMessages.Count > messageConfig.recentMessagesCount)
            {
                room.recentMessages.Dequeue();
            }

            // -> other users

            Dictionary<int, List<RoomUser>> dict = room.userDict
                .GroupBy(pair => pair.Value.gatewayServiceId, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            var broadcast = new MsgAChatMessage();
            broadcast.message = message;

            foreach (var pair in dict)
            {
                int gatewayServiceId = pair.Key;
                List<RoomUser> roomUsers = pair.Value;

                long[] userIds = roomUsers.Select(x => x.userId).ToArray();
                e = this.service.gatewayServiceProxy.BroadcastToClient(gatewayServiceId, userIds, MsgType.AChatMessage, broadcast);
                if (e == ECode.NotConnected)
                {
                    this.service.logger.Warn($"{this.msgType} gatewayServiceId {gatewayServiceId} is not connected");
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendSceneChat msg, ECode e, ResRoomSendSceneChat res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}