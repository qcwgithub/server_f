using Data;

namespace Script
{
    public class GatewayConnectionCallbackScript : ConnectionCallbackScript
    {
        public readonly GatewayService gatewayService;
        public GatewayConnectionCallbackScript(Server server, GatewayService gatewayService) : base(server, gatewayService)
        {
            this.gatewayService = gatewayService;
        }

        public override void OnMsg(IConnection connection, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            if (connection is GatewayUserConnection gatewayUserConnection)
            {
                ServiceType? serviceType = Forwarding.G_to_S(this.gatewayService, gatewayUserConnection, msgType, msgBytes, reply);
                if (serviceType != null)
                {
                    return;
                }
            }
            else if (connection is IServiceConnection serviceConnection)
            {
                if (Forwarding.G_from_S(this.gatewayService, msgType, msgBytes, reply))
                {
                    return;
                }
            }

            base.OnMsg(connection, seq, msgType, msgBytes, reply);
        }

        public override void OnLocalMsg(IConnection connection, int seq, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            if (connection is GatewayUserConnection gatewayUserConnection)
            {
                ServiceType? serviceType = LocalForwarding.G_to_S(this.gatewayService, gatewayUserConnection, msgType, msg, reply);
                if (serviceType != null)
                {
                    return;
                }
            }
            else if (connection is IServiceConnection serviceConnection)
            {
                if (LocalForwarding.G_from_S(this.gatewayService, msgType, msg, reply))
                {
                    return;
                }
            }

            base.OnLocalMsg(connection, seq, msgType, msg, reply);
        }
    }
}