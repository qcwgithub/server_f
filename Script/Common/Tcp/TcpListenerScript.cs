using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScript : ServiceScript<Service>, ITcpListenerCallback
    {
        public readonly bool forClient;
        public TcpListenerScript(Server server, Service service, bool forClient) : base(server, service)
        {
            this.forClient = forClient;
        }

        public void LogError(string str)
        {
            this.service.logger.Error(str);
        }

        public void LogInfo(string str)
        {
            this.service.logger.Info(str);
        }

        public void OnAcceptComplete(TcpListenerData tcpListener, SocketAsyncEventArgs e)
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

            new SocketConnection(this.service.data, socket, this.forClient);
        }
    }
}