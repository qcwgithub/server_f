using Data;

namespace Script
{
    public class GatewayConnectionCallbackScriptForS : ConnectionCallbackScript
    {
        public readonly GatewayService gatewayService;
        public GatewayConnectionCallbackScriptForS(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            var serviceConnection = connection as IServiceConnection;
            if (serviceConnection == null) // when from tool
            {
                MyDebug.Assert(false);
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