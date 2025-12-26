using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    // 登记连接我的是哪个服务
    public class OnConnectorInfo<S> : Handler<S, MsgConnectorInfo, ResConnectorInfo>
        where S : Service
    {
        public OnConnectorInfo(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._ConnectorInfo;

        public override async Task<ECode> Handle(IConnection connection, MsgConnectorInfo msg, ResConnectorInfo res)
        {
            ConnectorInfo info = msg.connectorInfo;
            string message = string.Format("{0} ServiceType.{1} serviceId {2} this.service.data.state {3}",
                this.msgType, info.serviceType, info.serviceId, this.service.data.state);
            if (this.service.data.state == ServiceState.Started)
            {
                this.service.logger.Info(message);
            }
            else
            {
                this.service.logger.Error(message);
            }

            var pendingConnection = (PendingSocketConnection)connection;
            var serviceConnection = new SocketServiceConnection(info.serviceType, info.serviceId, pendingConnection.socket, false);
            this.service.data.SaveOtherServiceConnection(serviceConnection);

            return ECode.Success;
        }
    }
}