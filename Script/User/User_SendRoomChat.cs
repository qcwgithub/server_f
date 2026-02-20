using Data;

namespace Script
{
    [AutoRegister]
    public class User_SendRoomChat : Handler<UserService, MsgSendRoomChat, ResSendRoomChat>
    {
        public override MsgType msgType => MsgType.SendRoomChat;
        public User_SendRoomChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendRoomChat msg, ResSendRoomChat res)
        {
            ECode e = RoomUtils.CheckRoomId(msg.roomId);
            if (e != ECode.Success)
            {
                return ECode.InvalidRoomId;
            }

            e = RoomUtils.CheckRoomType(msg.roomType);
            if (e != ECode.Success)
            {
                return e;
            }

            e = ChatUtils.CheckChatMessageType(msg.chatMessageType);
            if (e != ECode.Success)
            {
                return e;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            e = this.service.roomScript.CheckSendRoomChat(user, msg);
            if (e != ECode.Success)
            {
                return e;
            }

            UserInfo userInfo = user.userInfo;

            MyResponse r;

            stObjectLocation location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotExist;
            }

            var msgR = new MsgRoomSendChat();
            msgR.roomId = msg.roomId;
            msgR.userId = user.userId;
            msgR.type = msg.chatMessageType;
            msgR.content = msg.content;
            msgR.userName = user.userInfo.userName;
            msgR.avatarIndex = user.userInfo.avatarIndex;
            msgR.clientMessageId = msg.clientMessageId;
            msgR.imageContent = msg.imageContent;
            msgR.roomType = msg.roomType;

            r = await this.service.roomServiceProxy.SendChat(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resR = r.CastRes<ResRoomSendChat>();

            await Task.Delay(1000);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSendRoomChat msg, ECode e, ResSendRoomChat res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}