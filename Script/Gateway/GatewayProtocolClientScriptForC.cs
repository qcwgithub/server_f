using Data;

namespace Script
{
    public class GatewayProtocolClientScriptForC : ProtocolClientScript
    {
        public GatewayProtocolClientScriptForC(Server server, Service service) : base(server, service)
        {
        }

        public GatewayService gatewayService
        {
            get
            {
                return (GatewayService)this.service;
            }
        }

        public override void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var connection = (GatewayUserConnection)data.customData;

            ServiceType? serviceType = Forwarding.TryForwardClientMessage(this.gatewayService, connection, msgType, msgBytes, reply);
            if (serviceType == null)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}