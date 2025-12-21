namespace Data
{
    public class GatewayUserConnection : DirectConnection
    {
        public readonly GatewayUser user;

        public GatewayUserConnection(ProtocolClientData socket, GatewayUser user) : base(socket, false)
        {
            this.user = user;
        }
    }
}