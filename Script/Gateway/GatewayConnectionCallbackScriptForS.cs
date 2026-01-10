using Data;

namespace Script
{
    public class GatewayConnectionCallbackScriptForS : ConnectionCallbackScriptForS
    {
        public readonly GatewayService gatewayService;
        public GatewayConnectionCallbackScriptForS(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            var serviceConnection = connection as ServiceConnection;
            if (serviceConnection == null) // when from tool
            {
                base.OnMsg(connection, seq, msgType, msgBytes, reply);
                return;
            }

            bool b = Forwarding.G_from_S(this.gatewayService, msgType, msgBytes, reply);
            if (!b)
            {
                base.OnMsg(connection, seq, msgType, msgBytes, reply);
            }
        }
    }
}