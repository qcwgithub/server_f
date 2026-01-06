using Data;

namespace Script
{
    public class User_EnterRoom : Handler<UserService, MsgEnterRoom, ResEnterRoom>
    {
        public override MsgType msgType => MsgType.EnterRoom;
        public User_EnterRoom(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgEnterRoom msg, ResEnterRoom res)
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

            if (msg.roomId == user.roomId)
            {
                return ECode.Success;
            }

            MyResponse r;

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

                r = await this.service.roomServiceProxy.UserLeave(location.serviceId, msgLeave);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                user.roomId = 0;
            }

            location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                var msgLoad = new MsgRoomManagerLoadRoom();
                msgLoad.roomId = msg.roomId;

                r = await this.service.roomManagerServiceProxy.LoadRoom(msgLoad);
                if (r.e == ECode.Success)
                {
                    var resLoad = r.CastRes<ResRoomManagerLoadRoom>();
                    location = resLoad.location;

                    this.service.roomLocator.CacheLocation(msg.roomId, location);
                }
                else if (r.e == ECode.Retry)
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
                    return r.e;
                }
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

        public override void PostHandle(MessageContext context, MsgEnterRoom msg, ECode e, ResEnterRoom res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}