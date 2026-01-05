namespace Data
{
    public class SocketConnection : IConnection
    {
        public ProtocolClientData socket;
        public readonly bool isConnector;
        public SocketConnection(ProtocolClientData socket, bool isConnector)
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

        public void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {
            ArraySegment<byte> msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);
            this.socket.SendBytes(msgType, msgBytes, cb, pTimeoutS);
        }
    }
}