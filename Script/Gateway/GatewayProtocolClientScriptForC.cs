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

        public override void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            GatewayUserConnection? gatewayUserConnection = data.customData as GatewayUserConnection;
            if (gatewayUserConnection != null)
            {
                ServiceType? serviceType = Forwarding.G_to_S(this.gatewayService, gatewayUserConnection, msgType, msgBytes, reply);
                if (serviceType != null)
                {
                    return;
                }
            }

            base.ReceiveFromNetwork(data, seq, msgType, msgBytes, reply);
        }
    }
}