namespace Data
{
    public class GatewayUserConnection : DirectConnection
    {
        public GatewayUserConnection(ProtocolClientData socket) : base(socket, false)
        {

        }
    }
}