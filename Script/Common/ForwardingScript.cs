using Data;

namespace Script
{
    public class ForwardingScript : ServiceScript<Service>, ISendClientMessageThroughGateway
    {
        public ForwardingScript(Server server, Service service) : base(server, service)
        {
        }

        public void S_to_G(IServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? replyFromC)
        {
            Forwarding.S_to_G(serviceConnection, userId, msgType, msg, replyFromC);
        }
    }
}