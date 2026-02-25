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

            var friendChatRoom = await this.service.LockRoom<FriendChatRoom>(msg.roomId, context);
            if (friendChatRoom == null)
            {
                return ECode.RoomNotExist;
            }

            long now = TimeUtils.GetTime();
            e = this.service.chatScript.CheckChatTooFast(friendChatRoom, messageConfig, msg.userId, now);
            if (e != ECode.Success)
            {
                return e;
            }

            //// ok

            // last send
            this.service.chatScript.WriteChatStamp(friendChatRoom, messageConfig, msg.userId, now);

            // create message
            long seq = ++friendChatRoom.friendChatRoomInfo.messageSeq;
            ChatMessage message = RoomChatScript.CreateChatMessage(
                seq: seq,
                roomId: msg.roomId,
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

            long friendUserId = friendChatRoom.GetOtherUserId(msg.userId);

            // -> redis
            await this.server.friendChatMessagesRedis.Add(message);

            // -> broadcast
            stObjectLocation location = await this.service.userLocator.GetLocation(friendUserId);
            if (location.IsValid())
            {
                await this.service.userServiceProxy.ReceiveChatMessage(location.serviceId, new MsgReceiveChatMessage
                {
                    userId = friendUserId,
                    message = message,
                });
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