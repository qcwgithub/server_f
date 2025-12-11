namespace Data
{
    public class DirectConnection : IConnection
    {
        public ProtocolClientData socket;
        
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
        public void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            this.socket.SendBytes(msgType, msg, cb, pTimeoutS);
        }
    }
}