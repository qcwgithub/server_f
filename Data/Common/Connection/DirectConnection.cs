namespace Data
{
    public class DirectConnection : IConnection
    {
        public ProtocolClientData socket;
        public readonly bool isConnector;
        public DirectConnection(ProtocolClientData socket, bool isConnector)
        {
            this.socket = socket;
            this.socket.customData = this;
            this.isConnector = isConnector;
        }

        public bool isAcceptor
        {
            get
            {
                return !this.isConnector;
            }
        }

        public void Connect()
        {
            this.socket.Connect();
        }

        public bool IsConnecting()
        {
            return this.socket.IsConnecting();
        }

        public bool IsConnected()
        {
            return this.socket.IsConnected();
        }

        public int GetConnectionId()
        {
            return this.socket.GetSocketId();
        }

        public bool IsClosed()
        {
            return this.socket.IsClosed();
        }

        public void Close(string reason)
        {
            this.socket.Close(reason);
        }

        public string? closeReason
        {
            get
            {
                return this.socket.closeReason;
            }
        }

        public void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            this.socket.SendBytes(msgType, msg, cb, pTimeoutS);
        }
    }
}