using Data;

namespace Script
{
    public class _User_ResetName : Handler<UserService, MsgResetName, ResResetName>
    {
        public override MsgType msgType => MsgType._User_ResetName;
        public _User_ResetName(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgResetName msg, ResResetName res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId}");

            ECode e;
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                (e, user) = await this.service.ss.SimulateUserLogin(msg.userId, UserDestroyUserReason.SimulateLogin_ResetName);
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            user.userInfo.userName = UserManagerServiceScript.DefaultUserName(user.userInfo.createTimeS);
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgResetName msg, ECode e, ResResetName res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}