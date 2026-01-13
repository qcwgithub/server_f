using Data;

namespace Script
{
    public class OnShutdown<S> : Handler<S, MsgShutdown, ResShutdown>
        where S : Service
    {
        public OnShutdown(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Shutdown;

        public override async Task<ECode> Handle(MessageContext context, MsgShutdown msg, ResShutdown res)
        {
            return await this.service.Shutdown(msg.force);
        }
    }
}