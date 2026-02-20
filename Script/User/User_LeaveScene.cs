using Data;

namespace Script
{
    [AutoRegister]
    public class User_LeaveScene : Handler<UserService, MsgLeaveScene, ResLeaveScene>
    {
        public override MsgType msgType => MsgType.LeaveScene;
        public User_LeaveScene(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgLeaveScene msg, ResLeaveScene res)
        {
            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }
            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            if (user.publicRoomId == 0)
            {
                return ECode.Success;
            }

            stObjectLocation location = await this.service.roomLocator.GetLocation(user.publicRoomId);
            if (!location.IsValid())
            {
                return ECode.RoomLocationNotExist;
            }

            var msgR = new MsgRoomUserLeave();
            msgR.userId = user.userId;
            msgR.roomId = msg.roomId;

            var r = await this.service.roomServiceProxy.UserLeave(location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            user.publicRoomId = 0;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgLeaveScene msg, ECode e, ResLeaveScene res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}