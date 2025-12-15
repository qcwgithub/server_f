using System.Net.Sockets;
using Data;

namespace Script
{
    public class GatewayTcpListenerScriptForC : TcpListenerScript
    {
        public GatewayTcpListenerScriptForC(Server server, Service service) : base(server, service)
        {
        }

        protected override IConnection CreateConnection(TcpClientData tcpClientData)
        {
            var connection = new GatewayUserConnection(tcpClientData);
            return connection;
        }
    }
}