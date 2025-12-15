using Data;

namespace Script
{
    public class GatewayTcpListenerScript : TcpListenerScript
    {
        public GatewayTcpListenerScript(Server server, Service service) : base(server, service)
        {
        }
        
        protected override IConnection CreateConnection(TcpClientData tcpClientData)
        {
            return new GatewayUserConnection(tcpClientData);    
        }
    }
}