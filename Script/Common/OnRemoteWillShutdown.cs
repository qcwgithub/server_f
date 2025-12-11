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

        public override async Task<ECode> Handle(IConnection _connection, MsgRemoteWillShutdown msg, ResRemoteWillShutdown res)
        {
            ServiceConnection connection = (ServiceConnection)_connection;
            this.service.logger.InfoFormat("{0} {1}", this.msgType, connection.serviceTypeAndId == null ? "null" : connection.serviceTypeAndId.Value.ToString());
            connection.remoteWillShutdown = true;
            return ECode.Success;
        }
    }
}