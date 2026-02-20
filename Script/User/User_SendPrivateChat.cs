using Data;

namespace Script
{
    [AutoRegister]
    public class User_SendPrivateChat : Handler<UserService, MsgSendPrivateChat, ResSendPrivateChat>
    {
        public override MsgType msgType => MsgType.SendPrivateChat;
        public User_SendPrivateChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendPrivateChat msg, ResSendPrivateChat res)
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

            e = this.service.roomScript.CheckSendPrivateChat(user, msg);
            if (e != ECode.Success)
            {
                return e;
            }

            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.friendUserId);
            FriendInfo friendInfo = userInfo.friends[friendIndex];

            stObjectLocation location = await this.service.roomLocator.GetLocation(friendInfo.privateRoomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotExist;
            }

            MyResponse r;

            var msgR = new MsgRoomSendChat();
            msgR.roomId = friendInfo.privateRoomId;
            msgR.userId = user.userId;
            msgR.type = msg.chatMessageType;
            msgR.content = msg.content;
            msgR.userName = user.userInfo.userName;
            msgR.avatarIndex = user.userInfo.avatarIndex;
            msgR.clientMessageId = msg.clientMessageId;
            msgR.imageContent = msg.imageContent;
            msgR.roomType = RoomType.Private;

            r = await this.service.roomServiceProxy.SendChat(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resR = r.CastRes<ResRoomSendChat>();

            await Task.Delay(1000);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSendPrivateChat msg, ECode e, ResSendPrivateChat res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}