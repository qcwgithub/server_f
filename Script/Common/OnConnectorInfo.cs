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

        public override async Task<ECode> Handle(MessageContext context, MsgConnectorInfo msg, ResConnectorInfo res)
        {
            ConnectorInfo info = msg.connectorInfo;
            var connection = (SocketServiceConnection)context.connection;

            string message = string.Format("{0} {1}{2} this.service.data.state {3}",
                this.msgType, info.serviceType, info.serviceId, this.service.data.state);
            if (this.service.data.state == ServiceState.Started)
            {
                this.service.logger.Info(message);
            }
            else
            {
                this.service.logger.Error(message);
            }

            connection.serviceType = info.serviceType;
            connection.serviceId = info.serviceId;
            this.service.data.SaveOtherServiceConnection(connection);

            return ECode.Success;
        }
    }
}