
// 运维，GM功能
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_Action : UserHandler<MsgPSAction, ResPSAction>
    {
        public User_Action(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._ServerAction;

        public override async Task<ECode> Handle(IConnection connection, MsgPSAction msg, ResPSAction res)
        {
            this.logger.Info(this.msgType);
            var sd = this.service.sd;

            if (msg.allowNewUser != null)
            {
                bool pre = sd.allowNewUser;
                bool curr = msg.allowNewUser.Value;

                this.logger.InfoFormat("allowNewUser {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.allowNewUser = curr;
                }
            }

            if (msg.destroyTimeoutS != null)
            {
                int pre = sd.destroyTimeoutS;
                int curr = msg.destroyTimeoutS.Value;

                this.logger.InfoFormat("destroyTimeoutS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.destroyTimeoutS = curr;
                }
            }

            if (msg.saveIntervalS != null)
            {
                int pre = sd.saveIntervalS;
                int curr = msg.saveIntervalS.Value;

                this.logger.InfoFormat("saveIntervalS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.saveIntervalS = curr;
                }
            }

            return ECode.Success;
        }
    }
}