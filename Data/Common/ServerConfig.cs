namespace Data
{
    public class ServerConfig
    {
        public string purpose;
        public string logDir;
        public List<string> allowChannels;
        public int initRoomMessagesCount;
        public int maxRoomMessagesCount;

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

            if (this.initRoomMessagesCount <= 0 || this.maxRoomMessagesCount <= 0)
            {
                Program.LogStartError("this.initRoomMessagesCount <= 0 || this.maxRoomMessagesCount <= 0");
                return;
            }

            this.redisConfig.Init();
            this.mongoDBConfig.Init(this);
        }
    }
}