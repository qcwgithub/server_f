using Data;

namespace Script
{
    public class User_LeaveRoom : User_ClientHandler<MsgLeaveRoom, ResLeaveRoom>
    {
        public override MsgType msgType => MsgType.LeaveRoom;
        public User_LeaveRoom(Server server, UserService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle(UserConnection connection, MsgLeaveRoom msg, ResLeaveRoom res)
        {
            User user = connection.user;
            if (msg.roomId <= 0)
            {
                return ECode.InvalidParam;
            }

            if (user.roomId == 0)
            {
                return ECode.Success;
            }

            int serviceId = await this.service.roomLocator.GetOwningServiceId(user.roomId);
            if (serviceId == 0)
            {
                return ECode.RoomLocationNotFound;
            }

            var msgR = new MsgRoomUserLeave();
            msgR.userId = user.userId;
            msgR.roomId = msg.roomId;

            var r = await this.service.connectToRoomService.Request<MsgRoomUserLeave, ResRoomUserLeave>(serviceId, MsgType._Room_UserLeave, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            user.roomId = 0;
            
            return ECode.Success;
        }
    }
}