using Data;

namespace Script
{
    public class User_EnterRoom : User_ClientHandler<MsgEnterRoom, ResEnterRoom>
    {
        public override MsgType msgType => MsgType.EnterRoom;
        public User_EnterRoom(Server server, UserService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle(UserConnection connection, MsgEnterRoom msg, ResEnterRoom res)
        {
            User user = connection.user;
            if (msg.roomId <= 0)
            {
                return ECode.InvalidParam;
            }

            if (msg.roomId == user.roomId)
            {
                return ECode.AlreadyIs;
            }

            if (user.roomId != 0)
            {
                // leave first
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

                // ResRoomUserLeave resR = r.res;
            }

            {
                int serviceId = await this.service.roomLocator.GetOwningServiceId(msg.roomId);
                if (serviceId == 0)
                {
                    return ECode.RoomLocationNotFound;
                }

                var msgR = new MsgRoomUserEnter();
                msgR.userId = user.userId;
                msgR.roomId = msg.roomId;

                var r = await this.service.connectToRoomService.Request<MsgRoomUserEnter, ResRoomUserEnter>(serviceId, MsgType._Room_UserEnter, msgR);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                user.roomId = msg.roomId;

                // ResRoomUserEnter resR = r.res;
            }
            
            return ECode.Success;
        }
    }
}