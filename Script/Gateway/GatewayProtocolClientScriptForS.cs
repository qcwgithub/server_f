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

        public override void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var serviceConnection = (ServiceConnection)data.customData;

            bool b = Forwarding.GatewayTryForwardClientMessageToClient(this.gatewayService, msgType, msgBytes, reply);
            if (!b)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}