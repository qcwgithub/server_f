namespace Data
{
    public sealed class User
    {
        public bool destroying;

        public Profile profile { get; set; }
        public bool isNewProfile;
        public Random random { get; set; }
        public Dictionary<string, int> string2intDict { get; } = new Dictionary<string, int>();
        public HashSet<string> stringSet { get; } = new HashSet<string>();

        public ProtocolClientData socket;
        public bool IsSocketConnected()
        {
            return this.socket != null && this.socket.IsConnected();
        }

        public long userId = 0;
        public log4net.ILog logger;
        public ITimer destroyTimer;

        public int onlineTimeS;
        public int offlineTimeS;

        public ITimer saveTimer;

        //// 2 ////
        public Profile lastProfile;

        public bool battlePending;

        public bool isGm = false;

        Dictionary<string, int> autoRequesting = new Dictionary<string, int>();
        public bool IsAutoRequesting(string key, int nowS)
        {
            if (!this.autoRequesting.TryGetValue(key, out int timeS))
            {
                return false;
            }

            if (nowS - timeS >= 10)
            {
                this.autoRequesting.Remove(key);
                return false;
            }

            return true;
        }

        public void SetAutoRequesting(string key, bool reqeusting, int timeS)
        {
            if (!reqeusting)
            {
                this.autoRequesting.Remove(key);
            }
            else
            {
                this.autoRequesting[key] = timeS;
            }
        }

        public Dictionary<MsgType, int> processingMsgs { get; } = new Dictionary<MsgType, int>();
        public int accumDelayLoginS; // 已经延迟登录多久了

        public List<int> lastMessageSeqs { get; } = new List<int>();
    }
}