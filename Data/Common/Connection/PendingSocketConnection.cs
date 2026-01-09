namespace Data
{
    public class PendingSocketConnection : SocketConnection
    {
        public readonly bool fromS;
        public PendingSocketConnection(ServiceData serviceData, ProtocolClientData socket, bool fromS) : base(serviceData, socket, false)
        {
            this.fromS = fromS;
        }
    }
}