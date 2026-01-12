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

        public virtual void OnAcceptComplete(TcpListenerData tcpListener, SocketAsyncEventArgs e)
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

            if (!tcpListener.forClient)
            {
                new SocketServiceConnection(this.service.data, socket, false);
            }
            else
            {
                MyDebug.Assert(false);
            }
        }
    }
}