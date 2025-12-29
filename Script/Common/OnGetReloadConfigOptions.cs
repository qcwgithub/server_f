using Data;

namespace Script
{
    public class OnGetReloadConfigOptions<S> : Handler<S, MsgGetReloadConfigOptions, ResGetReloadConfigOptions>
        where S : Service
    {
        public OnGetReloadConfigOptions(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._GetReloadConfigOptions;

        public override async Task<ECode> Handle(MsgContext context, MsgGetReloadConfigOptions msg, ResGetReloadConfigOptions res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);
            
            res.files = new List<string>();

            this.service.data.GetReloadConfigOptions(res.files);

            return ECode.Success;
        }
    }
}