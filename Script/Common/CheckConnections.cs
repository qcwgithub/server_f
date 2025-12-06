using Data;

namespace Script
{
    // 连接的发起方，保持连接。每隔一段时间检查一下
    public class CheckConnections<S> : Handler<S, MsgCheckConnections>
        where S : Service
    {
        public override MsgType msgType => MsgType._CheckConnections;

        public override Task<MyResponse> Handle(ProtocolClientData _socket/* null */, MsgCheckConnections msg)
        {
            ServiceData sd = this.service.data;
            if (sd.connectToServiceTypes.Count == 0)
            {
                return ECode.Success.ToTask();
            }

            foreach (ServiceType to_serviceType in sd.connectToServiceTypes)
            {
                sd.current_resGetServiceConfigs.VisitServiceConfig(to_serviceType, sc =>
                {
                    if (sc.serviceId == sd.serviceId)
                    {
                        return;
                    }

                    this.service.GetOrConnectSocket(sc.serviceType, sc.serviceId, sc.inIp, sc.inPort);
                });
            }

            return ECode.Success.ToTask();
        }
    }
}