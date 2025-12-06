using System;
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    // 登记连接我的是哪个服务
    public class OnConnectorInfo<S> : Handler<S, MsgConnectorInfo>
        where S : Service
    {
        public override MsgType msgType => MsgType._ConnectorInfo;

        public override Task<MyResponse> Handle(ProtocolClientData socket, MsgConnectorInfo msg)
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

            this.service.data.SetOtherServiceSocket(info.serviceType, info.serviceId, socket);

            return ECode.Success.ToTask();
        }
    }
}