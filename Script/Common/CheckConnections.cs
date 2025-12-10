using Data;

namespace Script
{
    // 连接的发起方，保持连接。每隔一段时间检查一下
    public class CheckConnections<S> : Handler<S, MsgCheckConnections, ResCheckConnections>
        where S : Service
    {
        public CheckConnections(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._CheckConnections;

        public override async Task<ECode> Handle(ProtocolClientData _socket, MsgCheckConnections msg, ResCheckConnections res)
        {
            ServiceData sd = this.service.data;
            if (sd.connectToServiceTypes.Count == 0)
            {
                return ECode.Success;
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

            return ECode.Success;
        }
    }
}