namespace Data
{
    public class UserConnection : IConnection
    {
        public readonly int gatewayServiceId;
        public readonly User user;
        public MsgType msgProcessing;
        public UserConnection(int gatewayServiceId, User user)
        {
            this.gatewayServiceId = gatewayServiceId;
            this.user = user;
        }

        public User? GetUser()
        {
            return this.user;
        }

        public int GetConnectionId()
        {
            throw new NotImplementedException();
        }

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
    
        public void Close(string reason)
        {

        }

        public bool IsClosed()
        {
            return false;
        }

        public string? closeReason
        {
            get
            {
                return null;
            }
        }

        public void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            throw new NotImplementedException();
        }
    }
}