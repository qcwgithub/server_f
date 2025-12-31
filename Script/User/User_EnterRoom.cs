using Data;

namespace Script
{
    public class User_EnterRoom : User_ClientHandler<MsgEnterRoom, ResEnterRoom>
    {
        public override MsgType msgType => MsgType.EnterRoom;
        public User_EnterRoom(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgEnterRoom msg, ResEnterRoom res)
        {
            User user = context.user;
            if (msg.roomId <= 0)
            {
                return ECode.InvalidParam;
            }

            if (msg.roomId == user.roomId)
            {
                return ECode.Success;
            }

            stObjectLocation location;
            if (user.roomId != 0)
            {
                // leave first
                location = await this.service.roomLocator.GetLocation(user.roomId);
                if (!location.IsValid())
                {
                    return ECode.RoomLocationNotFound;
                }

                var msgLeave = new MsgRoomUserLeave();
                msgLeave.userId = user.userId;
                msgLeave.roomId = msg.roomId;

                var rLeave = await this.service.roomServiceProxy.UserLeave(location.serviceId, msgLeave);
                if (rLeave.e != ECode.Success)
                {
                    return rLeave.e;
                }

                user.roomId = 0;
            }

            location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                var msgLoad = new MsgRoomManagerLoadRoom();
                msgLoad.roomId = msg.roomId;

                var rLoad = await this.service.roomManagerServiceProxy.LoadRoom(msgLoad);
                if (rLoad.e == ECode.Success)
                {
                    var resLoad = rLoad.CastRes<ResRoomManagerLoadRoom>();
                    location = resLoad.location;

                    this.service.roomLocator.CacheLocation(msg.roomId, location);
                }
                else if (rLoad.e == ECode.Retry)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        // 1, 2, 3 seconds, total 6 seconds
                        await Task.Delay(i * 1000);

                        location = await this.service.roomLocator.GetLocation(msg.roomId);
                        if (location.IsValid())
                        {
                            break;
                        }
                    }
                    return ECode.RetryFailed;
                }
                else
                {
                    return rLoad.e;
                }
            }

            var msgEnter = new MsgRoomUserEnter();
            msgEnter.userId = user.userId;
            msgEnter.roomId = msg.roomId;

            var rEnter = await this.service.roomServiceProxy.UserEnter(location.serviceId, msgEnter);
            if (rEnter.e != ECode.Success)
            {
                return rEnter.e;
            }

            user.roomId = msg.roomId;

            return ECode.Success;
        }
    }
}