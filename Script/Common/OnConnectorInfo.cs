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
            var connection = (SocketServiceConnection)context.connection;
            if (msg.isCommand)
            {
                connection.isCommand = true;
            }
            else
            {
                ConnectorInfo info = msg.connectorInfo;

                if (this.service.data.state == ServiceState.Started)
                {
                    this.service.logger.Info($"{this.msgType} {info.serviceType}{info.serviceId} my state {this.service.data.state}");
                }
                else
                {
                    this.service.logger.Error($"{this.msgType} {info.serviceType}{info.serviceId} my state {this.service.data.state}");
                }

                connection.serviceType = info.serviceType;
                connection.serviceId = info.serviceId;
                this.service.data.SaveOtherServiceConnection(connection);
            }

            return ECode.Success;
        }
    }
}