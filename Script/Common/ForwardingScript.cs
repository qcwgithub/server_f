using Data;

namespace Script
{
    public class ForwardingScript : ServiceScript<Service>, ISendClientMessageThroughGateway
    {
        public ForwardingScript(Server server, Service service) : base(server, service)
        {
        }

        public void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, byte[] msg, Action<ECode, byte[]>? reply, int? pTimeoutS)
        {
            Forwarding.SendClientMessageThroughGateway(serviceConnection, userId, msgType, msg, reply, pTimeoutS);
        }
    }
}