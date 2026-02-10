using Data;

namespace Script
{
    public class User_SendRoomChat : Handler<UserService, MsgSendRoomChat, ResSendRoomChat>
    {
        public override MsgType msgType => MsgType.SendRoomChat;
        public User_SendRoomChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendRoomChat msg, ResSendRoomChat res)
        {
            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.roomId != user.roomId)
            {
                return ECode.WrongRoomId;
            }

            MyResponse r;

            stObjectLocation location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotFound;
            }

            var msgR = new MsgRoomSendChat();
            msgR.roomId = msg.roomId;
            msgR.userId = user.userId;
            msgR.type = msg.chatMessageType;
            msgR.content = msg.content;
            msgR.userName = user.userInfo.userName;
            msgR.avatarIndex = user.userInfo.avatarIndex;
            msgR.clientMessageId = msg.clientMessageId;

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