using Data;

namespace Script
{
    public class ForwardingScript : ServiceScript<Service>, ISendClientMessageThroughGateway
    {
        public ForwardingScript(Server server, Service service) : base(server, service)
        {
        }

        public void S_to_G(IServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? reply)
        {
            if (serviceConnection is InProcessServiceConnection)
            {
                LocalForwarding.S_to_G(serviceConnection, userId, msgType, msg, reply);
            }
            else
            {
                Forwarding.S_to_G(serviceConnection, userId, msgType, msg, reply);
            }
        }
    }
}