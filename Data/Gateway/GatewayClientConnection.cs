namespace Data
{
    public class GatewayClientConnection : DirectConnection
    {
        public GatewayClientConnection(ProtocolClientData socket) : base(socket, false)
        {

        }
    }
}