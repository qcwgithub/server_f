using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScriptForS : TcpListenerScript
    {
        public TcpListenerScriptForS(Server server, Service service) : base(server, service)
        {
        }

        protected override IConnection CreateConnection(TcpClientData tcpClientData)
        {
            var connection = new ServiceConnection(tcpClientData);
            return connection;
        }
    }
}