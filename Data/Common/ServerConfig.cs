namespace Data
{
    public class ServerConfig
    {
        public string purpose;
        public string logDir;
        public List<string> allowChannels;

        public class MessageConfig
        {
            public int initMessagesCount;
            public int maxMessagesCount;

            // valid range [minLength, maxLength]
            public int minLength;
            public int maxLength;
            public int minIntervalMs;
            public int periodMs;
            public int periodMaxCount;

            public void Init()
            {
                if (this.initMessagesCount <= 0 || this.maxMessagesCount <= 0)
                {
                    Program.LogStartError("this.initMessagesCount <= 0 || this.maxMessagesCount <= 0");
                    return;
                }

                if (this.minLength <= 0 || this.maxLength <= 0)
                {
                    Program.LogStartError("this.minLength <= 0 || this.maxLength <= 0");
                    return;
                }

                if (this.minIntervalMs <= 0)
                {
                    Program.LogStartError("this.minInterval <= 0");
                    return;
                }

                if (this.periodMs <= 0 || this.periodMaxCount <= 0)
                {
                    Program.LogStartError("this.periodMs <= 0 || this.periodMaxCount <= 0");
                    return;
                }
            }
        }
        public MessageConfig roomMessageConfig;

        public class UserNameConfig
        {
            public int minIntervalS;
            // valid range [minLength, maxLength]
            public int minLength;
            public int maxLength;

            public void Init()
            {
                if (this.minIntervalS <= 0)
                {
                    Program.LogStartError("this.minIntervalS <= 0");
                    return;
                }

                if (this.minLength <= 0 || this.maxLength <= 0)
                {
                    Program.LogStartError("this.minLength <= 0 || this.maxLength <= 0");
                    return;
                }
            }
        }
        public UserNameConfig userNameConfig;

        public class UserAvatarConfig
        {
            public int minIntervalS;
            // valid range [minIndex, maxIndex]
            public int minIndex;
            public int maxIndex;

            public void Init()
            {
                if (this.minIntervalS <= 0)
                {
                    Program.LogStartError("this.minIntervalS <= 0");
                    return;
                }

                if (this.minIndex < 0 || this.maxIndex < 0)
                {
                    Program.LogStartError("this.minIndex < 0 || this.maxIndex < 0");
                    return;
                }
            }
        }
        public UserAvatarConfig userAvatarConfig;

        ////
        public class RedisConfig
        {
            public string redisConn;
            public int db;
            public string requireMinVersion;

            public void Init()
            {
                if (string.IsNullOrEmpty(this.redisConn))
                {
                    Program.LogStartError("string.IsNullOrEmpty(this.redisConn)");
                    return;
                }
                if (this.db < 0)
                {
                    Program.LogStartError("this.db < 0");
                    return;
                }
                if (string.IsNullOrEmpty(this.requireMinVersion))
                {
                    Program.LogStartError("string.IsNullOrEmpty(this.requireMinVersion)");
                    return;
                }
            }
        }
        public RedisConfig redisConfig;

        ////
        public class MongoDBConfig
        {
            public string mongoDBConn;
            public string dbData;

            public void Init(ServerConfig serverConfig)
            {
                if (string.IsNullOrEmpty(this.mongoDBConn))
                {
                    Program.LogStartError("string.IsNullOrEmpty(this.mongoDBConn)");
                    return;
                }

                if (string.IsNullOrEmpty(this.dbData))
                {
                    Program.LogStartError("string.IsNullOrEmpty(this.dbData)");
                    return;
                }

                this.dbData = this.dbData.Replace("{purpose}", serverConfig.purpose);

                if (this.dbData.Contains('{'))
                {
                    Program.LogStartError("this.dbData.Contains('{')");
                    return;
                }
            }
        }
        public MongoDBConfig mongoDBConfig;

        public void Init()
        {
            if (string.IsNullOrEmpty(this.logDir))
            {
                Program.LogStartError("string.IsNullOrEmpty(this.logDir)");
                return;
            }

            this.logDir = this.logDir.Replace("{current}", Environment.CurrentDirectory);
            this.logDir = this.logDir.Replace("{home}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            this.logDir = this.logDir.Replace("{purpose}", this.purpose);
            if (this.logDir.Contains('{'))
            {
                Program.LogStartError("this.logDir.Contains('{')");
                return;
            }

            if (this.allowChannels == null || this.allowChannels.Count == 0)
            {
                Program.LogStartError("this.allowChannels == null || this.allowChannels.Count == 0");
                return;
            }

            this.roomMessageConfig.Init();
            this.userNameConfig.Init();
            this.userAvatarConfig.Init();
            this.redisConfig.Init();
            this.mongoDBConfig.Init(this);
        }
    }
}