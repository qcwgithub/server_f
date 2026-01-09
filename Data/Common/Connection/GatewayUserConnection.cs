namespace Data
{
    public class GatewayUserConnection : SocketConnection
    {
        public readonly GatewayUser user;

        public GatewayUserConnection(ServiceData serviceData, ProtocolClientData socket, GatewayUser user) : base(serviceData, socket, false)
        {
            this.user = user;
        }
    }
}