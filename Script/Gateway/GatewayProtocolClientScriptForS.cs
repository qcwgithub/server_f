using Data;

namespace Script
{
    public class GatewayProtocolClientScriptForS : ProtocolClientScriptForS
    {
        public readonly GatewayService gatewayService;
        public GatewayProtocolClientScriptForS(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            var serviceConnection = (ServiceConnection)data.customData;

            bool b = Forwarding.G_from_S(this.gatewayService, msgType, msgBytes, reply);
            if (!b)
            {
                base.ReceiveFromNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}