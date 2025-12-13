namespace Data
{
    public class UserConnection : IConnection
    {
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
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }
    
        public void Close(string reason)
        {
            throw new NotImplementedException();
        }

        public bool IsClosed()
        {
            throw new NotImplementedException();
        }

        public string? closeReason
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            throw new NotImplementedException();
        }

        public User? user;
        public long userId;
        public string? user_version;
        public long lastUserId;
        public MsgType msgProcessing;

        public void BindUser(User user)
        {
            if (!user.IsRealPrepareLogin(out MsgPrepareUserLogin? msgPreparePlayerLogin))
            {
                MyDebug.Assert(false);
            }

            this.user = user;
            this.userId = user.userId;
            this.user_version = msgPreparePlayerLogin!.version;
            this.lastUserId = user.userId;
        }

        public void UnbindUser()
        {
            this.user = null;
            this.userId = 0;
            this.user_version = string.Empty;
        }

        public User? GetUser()
        {
            return this.user;
        }
    }
}