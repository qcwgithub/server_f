using Data;
using MessagePack;

namespace Script
{
    [AutoRegister]
    public class Room_SendFriendChat : Handler<RoomService, MsgRoomSendFriendChat, ResRoomSendFriendChat>
    {
        public Room_SendFriendChat(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendFriendChat;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendFriendChat msg, ResRoomSendFriendChat res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} type {msg.type} content {msg.content}");

            ECode e = ChatUtils.CheckChatMessageType(msg.type);
            if (e != ECode.Success)
            {
                return e;
            }

            ServerConfig.MessageConfig messageConfig = this.server.data.serverConfig.privateMessageConfig;

            e = this.service.chatScript.CheckRoomSendPrivateChat(msg, messageConfig);
            if (e != ECode.Success)
            {
                return e;
            }

            var privateRoom = await this.service.LockRoom<FriendChatRoom>(msg.roomId, context);
            if (privateRoom == null)
            {
                return ECode.RoomNotExist;
            }

            long now = TimeUtils.GetTime();
            e = this.service.chatScript.CheckChatTooFast(privateRoom, messageConfig, msg.userId, now);
            if (e != ECode.Success)
            {
                return e;
            }

            //// ok

            // last send
            this.service.chatScript.WriteChatStamp(privateRoom, messageConfig, msg.userId, now);

            // create message
            var message = new ChatMessage();
            message.seq = ++privateRoom.friendChatInfo.seq;
            message.roomId = privateRoom.roomId;
            message.senderId = msg.userId;
            message.senderName = string.Empty;
            message.senderAvatar = string.Empty;
            message.type = msg.type;
            message.content = msg.content;
            message.timestamp = now;
            message.replyTo = 0;
            message.senderName = msg.userName;
            message.senderAvatarIndex = msg.avatarIndex;
            message.clientMessageId = msg.clientMessageId;
            message.status = ChatMessageStatus.Normal;
            message.imageContent = msg.imageContent;

            // -> redis
            await this.server.sceneMessagesRedis.Add(message);

            if (messageConfig.maxMessagesCount != -1 && message.seq % 100 == 0)
            {
                await this.server.sceneMessagesRedis.Trim(privateRoom.roomId, messageConfig.maxMessagesCount);
            }

            // -> other users
            foreach (PrivateRoomUser user in privateRoom.friendChatInfo.users)
            {
                if (user.userId != msg.userId)
                {
                    stObjectLocation location = await this.service.userLocator.GetLocation(user.userId);
                    if (location.IsValid())
                    {
                        await this.service.userServiceProxy.ReceiveChatMessage(location.serviceId, new MsgReceiveChatMessage
                        {
                            userId = user.userId,
                            message = message,
                        });
                    }
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendFriendChat msg, ECode e, ResRoomSendFriendChat res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}