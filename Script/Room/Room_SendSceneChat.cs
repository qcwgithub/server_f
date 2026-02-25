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

            var sceneRoom = await this.service.LockRoom<SceneRoom>(msg.roomId, context);
            if (sceneRoom == null)
            {
                return ECode.RoomNotExist;
            }

            SceneRoomUser? user = sceneRoom.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotInRoom;
            }

            long now = TimeUtils.GetTime();
            e = this.service.chatScript.CheckChatTooFast(sceneRoom, messageConfig, msg.userId, now);
            if (e != ECode.Success)
            {
                return e;
            }

            //// ok

            // last send
            this.service.chatScript.WriteChatStamp(sceneRoom, messageConfig, msg.userId, now);

            // create message
            long seq = ++sceneRoom.roomInfo.messageSeq;
            ChatMessage message = RoomChatScript.CreateChatMessage(
                seq: seq,
                roomId: sceneRoom.roomId,
                senderId: msg.userId,
                senderName: msg.userName,
                senderAvatar: string.Empty,
                type: msg.type,
                content: msg.content,
                timestamp: now,
                replyTo: 0,
                senderAvatarIndex: msg.avatarIndex,
                clientMessageId: msg.clientMessageId,
                status: ChatMessageStatus.Normal,
                imageContent: msg.imageContent
            );

            // -> redis
            await this.server.sceneMessagesRedis.Add(message);

            if (messageConfig.maxMessagesCount != -1 && message.seq % 100 == 0)
            {
                await this.server.sceneMessagesRedis.Trim(sceneRoom.roomId, messageConfig.maxMessagesCount);
            }

            // -> memory
            sceneRoom.recentMessages.Enqueue(message);
            while (sceneRoom.recentMessages.Count > messageConfig.recentMessagesCount)
            {
                sceneRoom.recentMessages.Dequeue();
            }

            // -> broadcast
            Dictionary<int, List<SceneRoomUser>> dict = sceneRoom.userDict
                .GroupBy(pair => pair.Value.gatewayServiceId, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            var broadcast = new MsgAChatMessage();
            broadcast.message = message;

            foreach (var pair in dict)
            {
                int gatewayServiceId = pair.Key;
                List<SceneRoomUser> roomUsers = pair.Value;

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