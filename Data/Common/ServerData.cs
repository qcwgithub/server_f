using MongoDB.Driver;
using StackExchange.Redis;
using MessagePack;
using MessagePack.Resolvers;

namespace Data
{
    public class ServerData
    {
        public static ServerData instance
        {
            get;
            private set;
        }

        public readonly Dictionary<string, string> arguments;

        public readonly ConfigLoader configLoader;
        public readonly Log4netCreation log4netCreation;

        public readonly ServerConfig serverConfig;

        public readonly GlobalServiceLocation globalServiceLocation;

        public int uncaughtExceptionCount;
        public readonly int timezoneOffset;

        // public readonly Data.WheelTimer.WheelTimer timer;

        public readonly IOThread ioThread;
        public List<long> error_feiShuSentTimeS = new List<long>();
        public List<long> fatal_feiShuSentTimeS = new List<long>();
        public Action<string, string> feiShuSendErrorMessage;
        public Action<string, string> feiShuSendFatalMessage;

        public readonly bool loggerNameWithServer;

        public readonly Dictionary<int, ServiceData> serviceDatas;
        public readonly List<ServiceTypeAndId> serviceTypeAndIds;
        public readonly TimerSData timerSData;
        public Random random = new Random();
        public readonly MessageConfigData msgConfigData;

        public ServerConfig.MongoDBConfig mongoDBConfig => this.serverConfig.mongoDBConfig;

        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0
        // ******** HttpClient is intended to be instantiated once per application, rather than per-use.
        HttpClient _httpClient;
        public HttpClient httpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;
            }
        }

        // https://mongodb.github.io/mongo-csharp-driver/2.13/getting_started/quick_tour/
        // The MongoClient instance actually represents a pool of connections to the database;
        // you will only need one instance of class MongoClient even with multiple threads.
        MongoClient _mongoClient;
        public MongoClient mongoClient
        {
            get
            {
                if (_mongoClient == null)
                {
                    // The mongoClient instance now holds a pool of connections to the server or servers specified in the connection string.
                    // you will only need one instance of class MongoClient even with multiple threads.
                    _mongoClient = new MongoClient(this.serverConfig.mongoDBConfig.mongoDBConn);
                }
                return _mongoClient;
            }
        }

        // public readonly IMongoDatabase accountDb;
        // public readonly IMongoDatabase playerDb;
        // public readonly IMongoDatabase logDb;

        // https://stackexchange.github.io/StackExchange.Redis/Basics
        // it is designed to be shared and reused between callers
        ConnectionMultiplexer _redis;
        public ConnectionMultiplexer redis
        {
            get
            {
                if (_redis == null)
                {
                    _redis = ConnectionMultiplexer.Connect(this.serverConfig.redisConfig.redisConn);
                }
                return _redis;
            }
        }

        public Version redisVersion;
        public IDatabase redis_db
        {
            get
            {
                return this.redis.GetDatabase(this.serverConfig.redisConfig.db);
            }
        }

        // init message pack
        static void InitializeMessagePack()
        {
            StaticCompositeResolver.Instance.Register(
                //  MessagePack.Resolvers.GeneratedResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
        }

        public ServerData(Dictionary<string, string> args)
        {
            instance = this;

            InitializeMessagePack();
            this.arguments = args;
            this.configLoader = new ConfigLoader();
            this.serverConfig = this.configLoader.LoadServerConfig();

            if (!this.configLoader.LoadAllServiceConfigs(
                out List<ServiceConfig> ori_allServiceConfigs,
                out string message))
            {
                throw new Exception(message);
            }

            ServiceConfig? global_sc = ori_allServiceConfigs.Find(x => x.serviceType == ServiceType.Global);
            if (global_sc == null)
            {
                Program.LogStartError("global_sc == null");
                return;
            }
            this.globalServiceLocation = new GlobalServiceLocation
            {
                serviceId = global_sc.serviceId,
                inIp = global_sc.inIp,
                inPort = global_sc.inPort
            };
            this.globalServiceLocation.Init();

            string servicesStr;
            if (!args.TryGetValue("services", out servicesStr))
            {
                Program.LogStartError($"missing 'services'");
                return;
            }

            if (servicesStr == "all")
            {
                List<ServiceConfig> list = ori_allServiceConfigs;

                this.serviceTypeAndIds = list
                    .Select(sc => sc.Tai())
                    .Where(tai => !tai.serviceType.IsCommand())
                    .ToList();
            }
            else
            {
                this.serviceTypeAndIds = servicesStr
                    .Split(',')
                    .Select(k => ServiceTypeAndId.FromString(k))
                    .Distinct()
                    .ToList();
            }

            // log4net
            List<string> loggerNamesToAdd;
            List<bool> releaseModelLogToConsole;
            // if (this.serverArgs.Count == 1)
            // {
            //     this.loggerNameWithServer = false;
            //     ServerArg baseServerArg = this.serverArgs.First().Value;
            //     loggerNamesToAdd = baseServerArg.serviceTypeAndIds.Select(tai => tai.ToString()).ToList();
            //     releaseModelLogToConsole = baseServerArg.serviceTypeAndIds.Select(tai => tai.serviceType.ReleaseModeLogToConsole()).ToList();
            // }
            // else
            {
                this.loggerNameWithServer = true;
                loggerNamesToAdd = new List<string>();
                releaseModelLogToConsole = new List<bool>();

                loggerNamesToAdd.AddRange(this.serviceTypeAndIds.Select(tai => tai.ToString()));
                releaseModelLogToConsole.AddRange(this.serviceTypeAndIds.Select(tai => tai.serviceType.ReleaseModeLogToConsole()));
            }

            this.log4netCreation = new Log4netCreation();
            this.log4netCreation.Create("my_log4net_repo", this.serverConfig.generalConfig.logDir, loggerNamesToAdd, releaseModelLogToConsole, this.configLoader.log4netConfigXml, true);

            this.timezoneOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            Console.WriteLine("Timezone Offset: " + this.timezoneOffset);

            //-----------------------------------------------------

            this.ioThread = new IOThread();
            this.msgConfigData = new MessageConfigData();

            // Console.WriteLine(this.redis.GetServer().Version);
            this.InitShutdownServiceOrder();

            this.serviceDatas = new Dictionary<int, ServiceData>();
            foreach (ServiceTypeAndId typeAndId in this.serviceTypeAndIds)
            {
                ServiceData sd = CreateServiceData(typeAndId);
                this.serviceDatas.Add(typeAndId.serviceId, sd);
            }

            this.timerSData = new TimerSData();
            this.timerSData.Start();
        }

        public List<ServiceType> shutdownServiceOrder { get; private set; }
        void InitShutdownServiceOrder()
        {
            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                List<ServiceType> connects = GetConnectToServiceIds(serviceType);

                for (ServiceType serviceType2 = 0; serviceType2 < ServiceType.Count; serviceType2++)
                {
                    if (serviceType == serviceType2)
                    {
                        continue;
                    }

                    List<ServiceType> connects2 = GetConnectToServiceIds(serviceType2);

                    bool b = connects.Contains(serviceType2);
                    bool b2 = connects2.Contains(serviceType);

                    if (b && b2)
                    {
                        // 互相连接
                        Program.LogStartError($"'{serviceType}' and '{serviceType2}' connect to each other");
                        return;
                    }
                }
            }

            var waits = new List<ServiceType>();
            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                waits.Add(serviceType);
            }
            var dones = new List<ServiceType>();

            while (waits.Count > 0)
            {
                foreach (ServiceType wait in waits)
                {
                    var connects = GetConnectToServiceIds(wait);

                    bool ok = true;
                    foreach (ServiceType c in connects)
                    {
                        if (!dones.Contains(c))
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        dones.Insert(0, wait);
                        waits.Remove(wait);
                        break;
                    }
                }
            }

            this.shutdownServiceOrder = dones;
        }

        static ServiceData CreateServiceData(ServiceTypeAndId typeAndId)
        {
            switch (typeAndId.serviceType)
            {
                case ServiceType.Gateway:
                    return new GatewayServiceData(typeAndId);

                case ServiceType.Db:
                    return new DbServiceData(typeAndId);

                case ServiceType.User:
                    return new UserServiceData(typeAndId);
                    
                case ServiceType.Global:
                    return new GlobalServiceData(typeAndId);

                case ServiceType.Command:
                    return new CommandServiceData(typeAndId);

                default:
                    throw new Exception("Not handled serviceType: " + typeAndId.serviceType);
            }
        }

        static List<ServiceType> GetConnectToServiceIds(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.Gateway:
                    return GatewayServiceData.s_connectToServiceIds;
    
                case ServiceType.Db:
                    return DbServiceData.s_connectToServiceIds;

                case ServiceType.User:
                    return UserServiceData.s_connectToServiceIds;

                case ServiceType.Global:
                    return GlobalServiceData.s_connectToServiceIds;

                case ServiceType.Command:
                    return CommandServiceData.s_connectToServiceIds;

                case ServiceType.UserManager:
                    return UserManagerServiceData.s_connectToServiceIds;

                case ServiceType.Room:
                    return RoomServiceData.s_connectToServiceIds;

                case ServiceType.RoomManager:
                    return RoomManagerServiceData.s_connectToServiceIds;

                default:
                    throw new Exception("Not handled serviceType: " + serviceType);
            }
        }
    }
}
