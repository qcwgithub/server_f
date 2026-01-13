using System.Net.Sockets;
using Data;

namespace Script
{
    public class GatewayTcpListenerScript : TcpListenerScript
    {
        public GatewayTcpListenerScript(Server server, Service service) : base(server, service)
        {

        }

        protected override void OnAccept(object? arg)
        {
            var acceptArg = (ITcpListenerCallback.OnAcceptArg)arg!;
            if (acceptArg.forClient)
            {
                new GatewayUserConnection(this.service.data, acceptArg.socket);
            }
            else
            {
                base.OnAccept(arg);
            }
        }
    }
}