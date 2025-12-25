namespace Data
{
    public class SelfConnection : IConnection
    {
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

        public void SendBytes(MsgType msgType, byte[] msg, ReplyCallback? cb, int? pTimeoutS)
        {
            throw new NotImplementedException();
        }
    }
}