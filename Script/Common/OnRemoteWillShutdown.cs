using Data;

namespace Script
{
    public class OnRemoteWillShutdown<S> : Handler<S, MsgRemoteWillShutdown, ResRemoteWillShutdown>
        where S : Service
    {
        public OnRemoteWillShutdown(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RemoteWillShutdown;

        public override async Task<ECode> Handle(MessageContext context, MsgRemoteWillShutdown msg, ResRemoteWillShutdown res)
        {
            if (context.connection is IServiceConnection serviceConnection)
            {
                this.service.logger.InfoFormat("{0} {1}", this.msgType, serviceConnection.identifierString);

                serviceConnection.remoteWillShutdown = true;
            }
            return ECode.Success;
        }
    }
}