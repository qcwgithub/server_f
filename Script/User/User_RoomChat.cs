using Data;

namespace Script
{
    public class User_RoomChat : User_ClientHandler<MsgRoomChat, ResRoomChat>
    {
        public override MsgType msgType => MsgType.RoomChat;
        public User_RoomChat(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgRoomChat msg, ResRoomChat res)
        {
            if (msg.roomId <= 0)
            {
                return ECode.InvalidParam;
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

            var msgEnter = new MsgRoomUserEnter();
            msgEnter.userId = user.userId;
            msgEnter.roomId = msg.roomId;
            msgEnter.gatewayServiceId = user.gatewayServiceId;

            r = await this.service.roomServiceProxy.UserEnter(location.serviceId, msgEnter);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            user.roomId = msg.roomId;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomChat msg, ECode e, ResRoomChat res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}