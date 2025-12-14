using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScript : ServiceScript<Service>, ITcpListenerCallback
    {
        public TcpListenerScript(Server server, Service service) : base(server, service)
        {
        }


        public void LogError(string str)
        {
            this.service.logger.Error(str);
        }

        public void LogInfo(string str)
        {
            this.service.logger.Info(str);
        }

        protected virtual IConnection CreateConnection(TcpClientData tcpClientData)
        {
            var serviceConnection = new ServiceConnection(tcpClientData);
            return serviceConnection;
        }

        public void OnAcceptComplete(TcpListenerData tcpListener, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // 有可能会走到这
                return;
            }

            var tcpClientData = new TcpClientData();
            Socket socket = e.AcceptSocket;
            socket.NoDelay = true;

            var connection = this.CreateConnection(tcpClientData);
            tcpClientData.customData = connection;

            tcpClientData.AcceptorInit(this.service.data, socket, tcpListener.isForClient);
        }
    }
}