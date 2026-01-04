using Data;

namespace Script
{
    public class User_Action : UserHandler<MsgUserServiceAction, ResUserServiceAction>
    {
        public User_Action(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_ServerAction;

        public override async Task<ECode> Handle(MessageContext context, MsgUserServiceAction msg, ResUserServiceAction res)
        {
            this.service.logger.Info(this.msgType);
            var sd = this.service.sd;

            if (msg.allowNewUser != null)
            {
                bool pre = sd.allowNewUser;
                bool curr = msg.allowNewUser.Value;

                this.service.logger.InfoFormat("allowNewUser {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.allowNewUser = curr;
                }
            }

            if (msg.saveIntervalS != null)
            {
                int pre = sd.saveIntervalS;
                int curr = msg.saveIntervalS.Value;

                this.service.logger.InfoFormat("saveIntervalS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.saveIntervalS = curr;
                }
            }

            return ECode.Success;
        }
    }
}