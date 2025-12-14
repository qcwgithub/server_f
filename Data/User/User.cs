namespace Data
{
    public sealed class User
    {
        public readonly UserInfo userInfo;
        public User(UserInfo userInfo)
        {
            this.userInfo = userInfo;
        }

        public long userId
        {
            get
            {
                return this.userInfo.userId;
            }
        }

        public UserConnection? connection;
        public bool IsConnected()
        {
            return this.connection != null && this.connection.IsConnected();
        }
        
        MsgSimulatePrepareUserLogin? msgSimulatePrepareUserLogin;
        MsgPrepareUserLogin? msgPrepareUserLogin;
        public void SetSimulatePrepareLogin(MsgSimulatePrepareUserLogin m)
        {
            this.msgSimulatePrepareUserLogin = m;
            MyDebug.Assert(this.msgPrepareUserLogin == null);
            this.msgPrepareUserLogin = null;
        }
        public void SetRealPrepareLogin(MsgPrepareUserLogin m)
        {
            this.msgPrepareUserLogin = m;
            this.msgSimulatePrepareUserLogin = null;
        }

        public bool IsRealPrepareLogin(out MsgPrepareUserLogin? msg)
        {
            msg = this.msgPrepareUserLogin;
            return msg != null;
        }

        public bool destroying;
        public ITimer? destroyTimer;

        public long onlineTimeS;
        public long offlineTimeS;

        public ITimer? saveTimer;

        //// 2 ////
        public UserInfo? lastUserInfo;

        public bool isGm = false;

        public Dictionary<MsgType, int> processingMsgs { get; } = new Dictionary<MsgType, int>();
        public int accumDelayLoginS; // 已经延迟登录多久了
    }
}