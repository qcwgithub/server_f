using Data;

namespace Script
{
    public class Gateway_Action : GatewayHandler<MsgGatewayServiceAction, ResGatewayServiceAction>
    {
        public Gateway_Action(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Gateway_ServerAction;

        public override async Task<ECode> Handle(MessageContext context, MsgGatewayServiceAction msg, ResGatewayServiceAction res)
        {
            this.service.logger.Info(this.msgType);
            var sd = this.service.sd;

            if (msg.destroyTimeoutS != null)
            {
                int pre = sd.destroyTimeoutS;
                int curr = msg.destroyTimeoutS.Value;

                this.service.logger.InfoFormat("destroyTimeoutS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.destroyTimeoutS = curr;
                }
            }

            return ECode.Success;
        }
    }
}