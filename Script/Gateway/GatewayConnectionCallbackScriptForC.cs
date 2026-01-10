using Data;

namespace Script
{
    public class GatewayConnectionCallbackScriptForC : ConnectionCallbackScript
    {
        public readonly GatewayService gatewayService;
        public GatewayConnectionCallbackScriptForC(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void OnMsg(IConnection connection, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            GatewayUserConnection? gatewayUserConnection = connection as GatewayUserConnection;
            if (gatewayUserConnection != null)
            {
                ServiceType? serviceType = Forwarding.G_to_S(this.gatewayService, gatewayUserConnection, msgType, msgBytes, reply);
                if (serviceType != null)
                {
                    return;
                }
            }

            base.OnMsg(connection, seq, msgType, msgBytes, reply);
        }
    }
}