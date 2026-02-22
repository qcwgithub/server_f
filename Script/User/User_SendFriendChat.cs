using Data;

namespace Script
{
    [AutoRegister]
    public class User_SendFriendChat : Handler<UserService, MsgSendFriendChat, ResSendFriendChat>
    {
        public override MsgType msgType => MsgType.SendFriendChat;
        public User_SendFriendChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendFriendChat msg, ResSendFriendChat res)
        {
            ECode e = ChatUtils.CheckChatMessageType(msg.chatMessageType);
            if (e != ECode.Success)
            {
                return e;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.friendUserId);
            if (friendIndex < 0)
            {
                return ECode.NotFriends;
            }

            FriendInfo friendInfo = userInfo.friends[friendIndex];

            stObjectLocation location = await this.service.roomLocator.GetLocation(friendInfo.roomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotExist;
            }

            MyResponse r;

            var msgR = new MsgRoomSendFriendChat();
            msgR.roomId = friendInfo.roomId;
            msgR.userId = user.userId;
            msgR.type = msg.chatMessageType;
            msgR.content = msg.content;
            msgR.userName = user.userInfo.userName;
            msgR.avatarIndex = user.userInfo.avatarIndex;
            msgR.clientMessageId = msg.clientMessageId;
            msgR.imageContent = msg.imageContent;

            r = await this.service.roomServiceProxy.SendFriendChat(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resR = r.CastRes<ResRoomSendFriendChat>();

            await Task.Delay(1000);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSendFriendChat msg, ECode e, ResSendFriendChat res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}