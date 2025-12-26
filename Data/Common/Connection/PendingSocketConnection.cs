namespace Data
{
    public class PendingSocketConnection : SocketConnection
    {
        public readonly bool fromS;
        public PendingSocketConnection(ProtocolClientData socket, bool fromS) : base(socket, false)
        {
            this.fromS = fromS;
        }
    }
}