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

        public ITimer? destroyTimer;

        public long onlineTimeS;
        public long offlineTimeS;

        public ITimer? saveTimer;

        public UserInfo? lastUserInfo;

        public bool isGm = false;
        public long roomId;

        public UserBriefInfo? lastBriefInfo;
    }
}