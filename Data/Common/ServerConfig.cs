namespace Data
{
    public class ServerConfig
    {
        public string purpose;
        ////
        public class GeneralConfig
        {
            public string logDir;
            public bool allowClientGM;
            public bool needCheckClientVersion;
            public List<string> allowChannels;
            public bool defaultOpen; // 默认是否对外开放。false 表示只有 GM 可以进

            public void Init(ServerConfig serverConfig)
            {
                if (string.IsNullOrEmpty(this.logDir))
                {
                    Program.LogStartError("string.IsNullOrEmpty(this.logDir)");
                    return;
                }

                this.logDir = this.logDir.Replace("{current}", Environment.CurrentDirectory);
                this.logDir = this.logDir.Replace("{home}", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                this.logDir = this.logDir.Replace("{purpose}", serverConfig.purpose);
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
            }
        }
        public GeneralConfig generalConfig;

        ////
        public class FeiShuConfig
        {
            //
            public int fatal_limitTimeS;
            public int fatal_limitCount;
            public string fatal_webhook;

            //
            public int error_limitTimeS;
            public int error_limitCount;
            public string error_webhook;

            //
            public bool event_enabled;
            public string event_webhook;

            //
            public bool chat_enabled;
            public string chat_webhook;
            public void Init()
            {

            }
        }

        public FeiShuConfig feiShuConfig;
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
            this.generalConfig.Init(this);
            this.feiShuConfig.Init();
            this.redisConfig.Init();
            this.mongoDBConfig.Init(this);
        }
    }
}