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
            User user = context.user;
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
    }
}