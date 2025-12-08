
// 运维，GM功能
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_Action : UserHandler<MsgPSAction, ResPSAction>
    {
        public override MsgType msgType => MsgType._ServerAction;

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgPSAction msg, ResPSAction res)
        {
            this.logger.Info(this.msgType);
            UserServiceData usData = this.usData;

            if (msg.allowNewUser != null)
            {
                bool pre = usData.allowNewUser;
                bool curr = msg.allowNewUser.Value;

                this.logger.InfoFormat("allowNewUser {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    usData.allowNewUser = curr;
                }
            }

            if (msg.destroyTimeoutS != null)
            {
                int pre = usData.destroyTimeoutS;
                int curr = msg.destroyTimeoutS.Value;

                this.logger.InfoFormat("destroyTimeoutS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    usData.destroyTimeoutS = curr;
                }
            }

            if (msg.saveIntervalS != null)
            {
                int pre = usData.saveIntervalS;
                int curr = msg.saveIntervalS.Value;

                this.logger.InfoFormat("saveIntervalS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    usData.saveIntervalS = curr;
                }
            }

            return ECode.Success;
        }
    }
}