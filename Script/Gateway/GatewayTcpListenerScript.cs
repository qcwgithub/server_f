using System.Net.Sockets;
using Data;

namespace Script
{
    public class GatewayTcpListenerScript : TcpListenerScript
    {
        public GatewayTcpListenerScript(Server server, Service service) : base(server, service)
        {

        }

        public override void OnAcceptComplete(TcpListenerData tcpListener, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // 有可能会走到这
                return;
            }

            Socket? socket = e.AcceptSocket;
            if (socket == null)
            {
                this.service.logger.Error("socket == null");
                return;
            }

            if (tcpListener.forClient)
            {
                new GatewayUserConnection(this.service.data, socket);
            }
            else
            {
                base.OnAcceptComplete(tcpListener, e);
            }
        }
    }
}