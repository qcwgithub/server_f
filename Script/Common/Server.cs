using System;
using System.Collections.Generic;
using Data;
using System.Threading.Tasks;
using System.Linq;

namespace Script
{
    // Script.dll 的入口
    // 全局，与业务无关
    public class Server : IServer
    {
        public static Version s_version { get; } = new Version(
        #region auto_version
            "0.4"
        #endregion auto_version
        );

        public ServerData data { get; private set; }
        public List<Service> services { get; protected set; }
        public readonly TimerScript timerScript;
        public readonly LockRedis lockRedis;
        public readonly FeiShuMessenger feiShuMessenger;
        // public abstract PersistenceTaskQueueRedis persistence_taskQueueRedis { get; }
        public readonly IMessageSerializer messageSerializer;
        public readonly IMessagePacker messagePacker;

        public Server()
        {
            this.messageSerializer = new BinaryMessageSerializer();
            this.messagePacker = new BinaryMessagePacker();

            this.timerScript = new TimerScript().Init(this);
            this.lockRedis = new LockRedis().Init(this);
            this.feiShuMessenger = new FeiShuMessenger().Init(this);
        }

        int seq;
        public int GetScriptDllSeq()
        {
            return this.seq;
        }
        public Version GetScriptDllVersion()
        {
            return s_version;
        }

        static string ExtractRedisVersionString(string redisInfo)
        {
            const string PREFIX = "redis_version:";
            int i = redisInfo.IndexOf(PREFIX);
            if (i < 0)
            {
                Console.WriteLine("redisInfo.IndexOf(\"redis_version:\") < 0");
                System.Environment.Exit(1);
                return null;
            }

            int j = redisInfo.IndexOf("\r\n", i);
            if (j < 0)
            {
                Console.WriteLine("redisInfo.IndexOf(\"\\r\\n\", i) < 0");
                System.Environment.Exit(1);
                return null;
            }

            string version = redisInfo.Substring(i + PREFIX.Length, j - i - PREFIX.Length);
            return version;
        }

        public async void Attach(Dictionary<string, string> args, ServerData data, int seq)
        {
            this.seq = seq;
            this.data = data;
            this.data.feiShuSendErrorMessage = this.feiShuMessenger.SendErrorMessage;
            this.data.feiShuSendFatalMessage = this.feiShuMessenger.SendFatalMessage;

            if (seq == 1)
            {
                string redisInfo = (await this.data.redis.GetDatabase(0).ExecuteAsync("INFO")).ToString();
                string redisVersionString = ExtractRedisVersionString(redisInfo);
                this.data.redisVersion = new Version(redisVersionString);
                Version requireMinRedisVersion = new Version(this.data.serverConfig.redisConfig.requireMinVersion);
                if (this.data.redisVersion < requireMinRedisVersion)
                {
                    Console.WriteLine("redisVersion({0}) < {1}", this.data.redisVersion, requireMinRedisVersion);
                    System.Environment.Exit(1);
                    return;
                }
            }


            this.data.timerSData.SetTimerCallback(this.OnTimer);

            this.CreateServices();
            this.AttachServices();

            if (seq == 1)
            {
                this.StartServices();
            }
        }

        void CreateServices()
        {
            this.services = new List<Service>();

            foreach (ServiceTypeAndId typeAndId in this.data.serviceTypeAndIds)
            {
                this.services.Add(this.CreateService(typeAndId));
            }
        }

        Service CreateService(ServiceTypeAndId typeAndId)
        {
            int serviceId = typeAndId.serviceId;
            switch (typeAndId.serviceType)
            {
                case ServiceType.Gateway:
                    return new GatewayService(this, serviceId);

                case ServiceType.Db:
                    return new DbService(this, serviceId);

                case ServiceType.User:
                    return new UserService(this, serviceId);

                case ServiceType.Global:
                    return new GlobalService(this, serviceId);

                case ServiceType.Command:
                    return new CommandService(this, serviceId);

                default:
                    throw new Exception("Not handled ServiceType: " + typeAndId.serviceType);
            }
        }

        void AttachServices()
        {
            foreach (var service in this.services)
            {
                service.Attach();
            }
        }

        void StartServices()
        {
            // 启动顺序与关闭顺序相反
            this.services.Sort((a, b) =>
            {
                int a_order = this.data.shutdownServiceOrder.IndexOf(a.data.serviceType);
                int b_order = this.data.shutdownServiceOrder.IndexOf(b.data.serviceType);
                return b_order - a_order;
            });

            foreach (var service in this.services)
            {
                service.dispatcher.DispatchLocal<MsgStart, ResStart>(null, MsgType._Start, new MsgStart());
            }
        }

        public async void Detach()
        {
            Console.WriteLine("**** Script.dll Detaching, {0}", this.GetScriptDllVersion());
            foreach (var service in this.services)
            {
                service.StartDetach();
                await service.Detach();
                service.EndDetach();
            }
            Console.WriteLine("**** Script.dll Detached, {0}", this.GetScriptDllVersion());
        }

        public async void OnServiceSetStateToShutdown()
        {
            bool allServicesReadyToShutdown = true;

            foreach (var kv in this.data.serviceDatas)
            {
                int serviceId = kv.Key;
                ServiceType serviceType = kv.Value.serviceType;
                if (kv.Value.state != ServiceState.ReadyToShutdown)
                {
                    // Console.WriteLine("!canExit " + serviceType);
                    allServicesReadyToShutdown = false;
                    break;
                }
            }

            if (!allServicesReadyToShutdown)
            {
                return;
            }

            // Stop entry businesses
            while (true)
            {
                int pendingCount = this.data.ioThread.PendingCount();
                Data.Program.LogInfo(string.Format("IO Thread pending {0} finished {1} total {2}", pendingCount,
                    this.data.ioThread.FinishedCount(),
                    this.data.ioThread.TotalCount()));
                if (pendingCount == 0)
                {
                    break;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }

            // Timer

            if (this.data.timerSData.timerDict.Count != 0)
            {
                Data.Program.LogError(string.Format("timer is still running! {0}", JsonUtils.stringify(this.data.timerSData.timerDict)), null);
            }

            await Task.Delay(100);

            foreach (var kv in this.data.serviceDatas)
            {
                await kv.Value.CloseAllConnections();
            }

            await Task.Delay(100);

            Data.Program.LogInfo("Exit now...");

            Environment.Exit(0);
        }

        public void OnTimer(int serviceId, MsgType msgType, object msg)
        {
            bool found = false;
            foreach (var service in this.services)
            {
                if (service.serviceId == serviceId)
                {
                    service.dispatcher.DispatchLocal<object, object>(null, msgType, msg);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Data.Program.LogError(string.Format("ScriptEntry.OnTimer !found serviceId({0}) msgType({1})", serviceId, msgType), null);
            }
        }

        public void RunOnIOThread(Action action)
        {
            this.data.ioThread.Run(action);
        }

        public async void HandleEvent(string event_)
        {
            if (event_ == "exit")
            {
                var allServices = new List<Service>(this.services);

                var order = this.data.shutdownServiceOrder;
                allServices.Sort((a, b) =>
                {
                    if (a.data.serviceType == b.data.serviceType)
                    {
                        return 0;
                    }
                    return order.IndexOf(a.data.serviceType) - order.IndexOf(b.data.serviceType);
                });

                // Console.WriteLine(JsonUtils.stringify(allServices.Select(s => s.data.serviceType.ToString()).ToArray()));
                // return;

                var msgShutdown = new MsgShutdown();
                msgShutdown.force = false;
                foreach (var baseService in allServices)
                {
                    await baseService.connectToSelf.Request<MsgShutdown, ResShutdown>(MsgType._Shutdown, msgShutdown);
                    await Task.Delay(100);
                }
            }
        }

        public void FrameStart(long frame)
        {
        }

        bool fpsStarted;
        DateTime fpsStartDt;
        int fps;
        public void FrameEnd(long frame)
        {
            if (!this.fpsStarted)
            {
                this.fpsStarted = true;
                this.fpsStartDt = DateTime.Now;
                this.fps = 0;
                return;
            }

            DateTime now = DateTime.Now;

            double ms = now.Subtract(this.fpsStartDt).TotalMilliseconds;
            if (ms < 1000)
            {
                this.fps++;
                return;
            }

#if DEBUG
            Console.Title = string.Format("Fps {0}", fps);
#endif
            foreach (var service in this.services)
            {
                service.OnFps(fps);
            }

            // reset
            this.fpsStartDt = DateTime.Now;
            this.fps = 0;
        }
    }
}
