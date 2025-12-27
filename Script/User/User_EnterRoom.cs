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
                return ECode.Success;
            }

            int serviceId;
            if (user.roomId != 0)
            {
                // leave first
                serviceId = await this.service.roomLocator.GetOwningServiceId(user.roomId);
                if (serviceId == 0)
                {
                    return ECode.RoomLocationNotFound;
                }

                var msgLeave = new MsgRoomUserLeave();
                msgLeave.userId = user.userId;
                msgLeave.roomId = msg.roomId;

                var rLeave = await this.service.connectToRoomService.Request<MsgRoomUserLeave, ResRoomUserLeave>(serviceId, MsgType._Room_UserLeave, msgLeave);
                if (rLeave.e != ECode.Success)
                {
                    return rLeave.e;
                }

                user.roomId = 0;
            }

            serviceId = await this.service.roomLocator.GetOwningServiceId(msg.roomId);
            if (serviceId == 0)
            {
                var msgLoad = new MsgLoadRoom();
                msgLoad.roomId = msg.roomId;

                var rLoad = await this.service.connectToRoomManagerService.Request<MsgLoadRoom, ResLoadRoom>(MsgType._RoomManager_LoadRoom, msgLoad);
                if (rLoad.e != ECode.Success)
                {
                    return rLoad.e;
                }

                ResLoadRoom resLoad = rLoad.res;
                this.service.roomLocator.SaveOwningServiceId(msg.roomId, resLoad.serviceId, resLoad.expiry);

                serviceId = resLoad.serviceId;
            }

            var msgEnter = new MsgRoomUserEnter();
            msgEnter.userId = user.userId;
            msgEnter.roomId = msg.roomId;

            var rEnter = await this.service.connectToRoomService.Request<MsgRoomUserEnter, ResRoomUserEnter>(serviceId, MsgType._Room_UserEnter, msgEnter);
            if (rEnter.e != ECode.Success)
            {
                return rEnter.e;
            }

            user.roomId = msg.roomId;

            return ECode.Success;
        }
    }
}