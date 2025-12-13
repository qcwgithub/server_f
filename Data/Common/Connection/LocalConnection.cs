namespace Data
{
    public class LocalConnection : IConnection
    {
        public void Connect()
        {
            throw new NotImplementedException();            
        }

        public bool IsConnecting()
        {
            return false;
        }

        public bool IsConnected()
        {
            return true;
        }

        public int GetConnectionId()
        {
            return 0;
        }

        public bool IsClosed()
        {
            return false;
        }

        public void Close(string reason)
        {
            
        }

        public string? closeReason
        {
            get
            {
                return string.Empty;
            }
        }

        public void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            throw new NotImplementedException();
        }
    }
}