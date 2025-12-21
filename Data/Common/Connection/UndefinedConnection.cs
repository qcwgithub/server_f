namespace Data
{
    public class UndefinedConnection : DirectConnection
    {
        public readonly bool fromS;
        public UndefinedConnection(ProtocolClientData socket, bool fromS) : base(socket, false)
        {
            this.fromS = fromS;
        }
    }
}