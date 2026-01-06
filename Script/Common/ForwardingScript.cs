using Data;

namespace Script
{
    public class ForwardingScript : ServiceScript<Service>, ISendClientMessageThroughGateway
    {
        public ForwardingScript(Server server, Service service) : base(server, service)
        {
        }

        public void S_to_G(ServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? reply, int? pTimeoutS)
        {
            Forwarding.S_to_G(serviceConnection, userId, msgType, msg, reply, pTimeoutS);
        }
    }
}