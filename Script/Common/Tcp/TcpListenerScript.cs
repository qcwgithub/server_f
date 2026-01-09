using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScript : ServiceScript<Service>, ITcpListenerCallback
    {
        public readonly bool forS;
        public TcpListenerScript(Server server, Service service, bool forS) : base(server, service)
        {
            this.forS = forS;
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

            var tcpClientData = new TcpClientData();
            Socket? socket = e.AcceptSocket;
            if (socket == null)
            {
                this.service.logger.Error("socket == null");
                return;
            }
            socket.NoDelay = true;

            var connection = new PendingSocketConnection(this.service.data, tcpClientData, this.forS);
            tcpClientData.customData = connection;

            tcpClientData.AcceptorInit(this.service.data, socket, tcpListener.isForClient);
        }
    }
}