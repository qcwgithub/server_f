using Data;

namespace Script
{
    public class OnResGetServiceConfigs<S> : Handler<S, A_ResGetServiceConfigs, ResNull>
        where S : Service
    {
        public OnResGetServiceConfigs(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Service_A_ResGetServiceConfigs;

        public override async Task<ECode> Handle(MessageContext context, A_ResGetServiceConfigs msg, ResNull res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);
            this.service.data.SaveServiceConfigs(msg.res);
            return ECode.Success;
        }
    }
}