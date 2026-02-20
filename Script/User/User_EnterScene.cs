using Data;

namespace Script
{
    [AutoRegister]
    public class User_EnterScene : Handler<UserService, MsgEnterScene, ResEnterScene>
    {
        public override MsgType msgType => MsgType.EnterScene;
        public User_EnterScene(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgEnterScene msg, ResEnterScene res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} lastMessageId {msg.lastMessageId}");

            ECode e = RoomUtils.CheckRoomId(msg.roomId);
            if (e != ECode.Success)
            {
                return ECode.InvalidRoomId;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            MyResponse r;

            stObjectLocation location;
            if (user.publicRoomId != 0 && user.publicRoomId != msg.roomId)
            {
                // leave first
                location = await this.service.roomLocator.GetLocation(user.publicRoomId);
                if (!location.IsValid())
                {
                    return ECode.RoomLocationNotExist;
                }

                var msgLeave = new MsgRoomUserLeave();
                msgLeave.userId = user.userId;
                msgLeave.roomId = msg.roomId;

                r = await this.service.roomServiceProxy.UserLeave(location.serviceId, msgLeave);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                user.publicRoomId = 0;
            }

            location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                var msgLoad = new MsgRoomManagerLoadRoom();
                msgLoad.roomId = msg.roomId;

                r = await this.service.roomManagerServiceProxy.LoadRoom(msgLoad);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                var resLoad = r.CastRes<ResRoomManagerLoadRoom>();
                location = resLoad.location;

                this.service.roomLocator.CacheLocation(msg.roomId, location);
            }

            var msgEnter = new MsgRoomUserEnter();
            msgEnter.userId = user.userId;
            msgEnter.roomId = msg.roomId;
            msgEnter.gatewayServiceId = user.connection.gatewayServiceId;
            msgEnter.lastMessageId = msg.lastMessageId;

            r = await this.service.roomServiceProxy.UserEnter(location.serviceId, msgEnter);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok

            user.publicRoomId = msg.roomId;

            var resEnter = r.CastRes<ResRoomUserEnter>();
            res.recentMessages = resEnter.recentMessages;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgEnterScene msg, ECode e, ResEnterScene res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}