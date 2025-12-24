using Data;

namespace Script
{
    public class GatewayProtocolClientScriptForC : ProtocolClientScript
    {
        public readonly GatewayService gatewayService;
        public GatewayProtocolClientScriptForC(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var connection = (GatewayUserConnection)data.customData;

            ServiceType? serviceType = Forwarding.GatewayTryForwardClientMessageToOtherService(this.gatewayService, connection, msgType, msgBytes, reply);
            if (serviceType == null)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}