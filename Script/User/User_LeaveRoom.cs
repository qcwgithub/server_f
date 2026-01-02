using Data;

namespace Script
{
    public class User_LeaveRoom : User_ClientHandler<MsgLeaveRoom, ResLeaveRoom>
    {
        public override MsgType msgType => MsgType.LeaveRoom;
        public User_LeaveRoom(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgLeaveRoom msg, ResLeaveRoom res)
        {
            User? user = await this.service.LockUser(context.userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }
            if (msg.roomId <= 0)
            {
                return ECode.InvalidParam;
            }

            if (user.roomId == 0)
            {
                return ECode.Success;
            }

            stObjectLocation location = await this.service.roomLocator.GetLocation(user.roomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotFound;
            }

            var msgR = new MsgRoomUserLeave();
            msgR.userId = user.userId;
            msgR.roomId = msg.roomId;

            var r = await this.service.roomServiceProxy.UserLeave(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            user.roomId = 0;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgLeaveRoom msg, ECode e, ResLeaveRoom res)
        {
            this.service.TryUnlockUser(context.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}