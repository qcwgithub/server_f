using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;

namespace Data
{
    public abstract class ServiceData :
        IProtocolClientCallbackProvider,
        ITcpListenerCallbackProvider,
        IHttpListenerCallbackProvider,
        IWebSocketListenerCallbackProvider
    {
        public bool replyServerTime = true;

        public ServiceType serviceType => this.serviceTypeAndId.serviceType;
        public int serviceId => this.serviceTypeAndId.serviceId;
        public readonly ServiceTypeAndId serviceTypeAndId;
        public int errorCount = 0;
        public int defaultDateTime = 0;
        public ServiceState state = ServiceState.Initing;
        public int msgSeq = 1;
        public int socketId = 90000;

        public Random random = new Random();
        public ILog logger;

        // tcp listener
        public ITcpListenerCallback tcpListenerCallback;
        public ITcpListenerCallback GetTcpListenerCallback() => this.tcpListenerCallback;

        // tcp client callback
        public IProtocolClientCallback tcpClientCallback;
        public IProtocolClientCallback GetProtocolClientCallback() => this.tcpClientCallback;

        public TcpListenerData tcpListenerForServer;
        public TcpListenerData tcpListenerForClient;

        // http listener
        public IHttpListenerCallback httpListenerCallback;
        public IHttpListenerCallback GetHttpListenerCallback() => this.httpListenerCallback;
        public HttpListenerData httpListenerDataForAll;

        public IWebSocketListenerCallback webSocketListenerCallback;
        public IWebSocketListenerCallback GetWebSocketListenerCallback() => this.webSocketListenerCallback;
        public WebSocketListenerData webSocketListenerDataForServer;
        public WebSocketListenerData webSocketListenerDataForClient;

        // 需要连接到哪些服务器
        public List<ServiceType> connectToServiceTypes = new List<ServiceType>();

        // 只存 Normal -> Normal 和 Group -> Group
        public Dictionary<int, ProtocolClientData> otherServiceSockets = new Dictionary<int, ProtocolClientData>();
        public List<ProtocolClientData>[] otherServiceSockets2 = new List<ProtocolClientData>[(int)ServiceType.Count];
        public void SetOtherServiceSocket(ServiceType serviceType, int serviceId, ProtocolClientData tcpClientData)
        {
            {
                if (this.otherServiceSockets.TryGetValue(serviceId, out ProtocolClientData old))
                {
                    if (old.IsConnected() || old.IsConnecting())
                    {
                        // Commander 不要报了
                        if (old.serviceTypeAndId != null && old.serviceTypeAndId.Value.serviceType.IsCommand())
                        {

                        }
                        else
                        {
                            this.logger.Error($"SetOtherServiceSocket tai {serviceType}{serviceId} old.IsConnected() || old.IsConnecting()");
                        }
                    }
                }
            }

            this.otherServiceSockets[serviceId] = tcpClientData;
            tcpClientData.serviceTypeAndId = new ServiceTypeAndId { serviceType = serviceType, serviceId = serviceId };

            var list = this.otherServiceSockets2[(int)serviceType];
            if (list == null)
            {
                list = this.otherServiceSockets2[(int)serviceType] = new List<ProtocolClientData>();
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var old = list[i];
                    if (old.IsConnected() || old.IsConnecting())
                    {
                        continue;
                    }

                    if (old.serviceTypeAndId == null || old.serviceTypeAndId.Value.serviceId != serviceId)
                    {
                        continue;
                    }

                    list.RemoveAt(i);
                    i--;
                }
            }

            if (list.IndexOf(tcpClientData) < 0)
            {
                list.Add(tcpClientData);
            }
        }
        public ProtocolClientData GetOtherServiceSocket(int serviceId)
        {
            ProtocolClientData socket;
            return this.otherServiceSockets.TryGetValue(serviceId, out socket) ? socket : null;
        }

        // 有没有被动连接还活着，要去掉 Command
        public virtual List<ServiceTypeAndId> GetPassivelyConnections()
        {
            var list = new List<ServiceTypeAndId>();

            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                if (this.connectToServiceTypes.Contains(serviceType))
                {
                    continue;
                }

                if (serviceType.IsCommand())
                {
                    continue;
                }

                List<ProtocolClientData> sockets = this.otherServiceSockets2[(int)serviceType];
                if (sockets == null || sockets.Count == 0)
                {
                    continue;
                }

                foreach (ProtocolClientData socket in sockets)
                {
                    if (socket.IsConnected() && socket.serviceTypeAndId != null)
                    {
                        list.Add(socket.serviceTypeAndId.Value);
                    }
                }
            }

            return list;
        }

        // 关闭所有主动发起的连接
        public virtual async Task CloseProactiveConnections()
        {
            var tasks = new List<Task>();

            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                if (!this.connectToServiceTypes.Contains(serviceType))
                {
                    continue;
                }

                List<ProtocolClientData> sockets = this.otherServiceSockets2[(int)serviceType];
                if (sockets == null || sockets.Count == 0)
                {
                    continue;
                }

                foreach (ProtocolClientData socket in sockets)
                {
                    if (socket.IsConnected())
                    {
                        tasks.Add(socket.SendAsync(MsgType._RemoteWillShutdown, null, pTimeoutS: 5));
                    }
                }
                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                List<int> serviceIds = new List<int>();
                foreach (ProtocolClientData socket in sockets)
                {
                    serviceIds.Add(socket.serviceTypeAndId.Value.serviceId);
                }

                foreach (ProtocolClientData socket in sockets)
                {
                    socket.Close("manual close");
                }
                sockets.Clear();

                foreach (int serviceId in serviceIds)
                {
                    this.otherServiceSockets.Remove(serviceId);
                }
            }
        }

        public virtual async Task CloseAllConnections()
        {
            // 主动关闭的也提前告知
            // -----------------------------------------------------------
            var tasks = new List<Task>();
            foreach (var kv in this.otherServiceSockets)
            {
                ProtocolClientData socket = kv.Value;
                if (socket.IsConnected())
                {
                    tasks.Add(socket.SendAsync(MsgType._RemoteWillShutdown, null, pTimeoutS: 5));
                }
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
            // -----------------------------------------------------------

            foreach (var kv in this.otherServiceSockets)
            {
                ProtocolClientData socket = kv.Value;
                socket.Close("manual close");
            }
            this.otherServiceSockets.Clear();

            foreach (var list in this.otherServiceSockets2)
            {
                if (list != null)
                    list.Clear();
            }
        }

        // showdown 时由于被他人连接而关闭不了时，先打标记
        public bool markedShutdown;
        public ITimer timer_shutdown;

        public Dictionary<string, int> intMap = new Dictionary<string, int>();
        public int GetInt(string key)
        {
            int value;
            return this.intMap.TryGetValue(key, out value) ? value : 0;
        }
        public void SetInt(string key, int value)
        {
            if (value == 0 && this.intMap.ContainsKey(key))
            {
                this.intMap.Remove(key);
                return;
            }
            this.intMap[key] = value;
        }

        public Dictionary<string, string> stringMap = new Dictionary<string, string>();
        public string GetString(string key)
        {
            string value;
            return this.stringMap.TryGetValue(key, out value) ? value : null;
        }
        public void SetString(string key, string value)
        {
            this.stringMap[key] = value;
        }

        public void ListenForServer_Tcp()
        {
            ref TcpListenerData d = ref this.tcpListenerForServer;
            int port = this.serviceConfig.inPort;

            MyDebug.Assert(d == null);

            d = new TcpListenerData() { isForClient = false, callbackProvider = this };
            this.logger.InfoFormat("ListenForServer_Tcp inPort {0}", port);
            d.Listen(port);
            d.StartAccept();

            // this.logger.InfoFormat("Listen for server on: " + port);
        }
        public void StopListenForServer_Tcp()
        {
            ref TcpListenerData d = ref this.tcpListenerForServer;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void ListenForClient_Tcp(int outPort)
        {
            ref TcpListenerData d = ref this.tcpListenerForClient;

            MyDebug.Assert(d == null);
            d = new TcpListenerData() { isForClient = true, callbackProvider = this };
            this.logger.InfoFormat("ListenForClient_Tcp outPort {0}", outPort);
            d.Listen(outPort);
            d.StartAccept();
        }
        public void StopListenForClient_Tcp()
        {
            ref TcpListenerData d = ref this.tcpListenerForClient;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void ListenForClient_Ws(string[] clientListener_wsPrefixes)
        {
            ref WebSocketListenerData d = ref this.webSocketListenerDataForClient;

            MyDebug.Assert(d == null);

            d = new WebSocketListenerData() { isForClient = true, callbackProvider = this };
            this.logger.InfoFormat("WebSocket listen for client on: " + JsonUtils.stringify(clientListener_wsPrefixes));
            d.Listen(clientListener_wsPrefixes);
            d.StartAccept();

        }
        public void StopListenForClient_Ws()
        {
            ref WebSocketListenerData d = ref this.webSocketListenerDataForClient;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void Listen_Http(string[] prefixes, List<string> httpAllowedIps)
        {
            ref HttpListenerData d = ref this.httpListenerDataForAll;

            MyDebug.Assert(d == null);

            d = new HttpListenerData() { callbackProvider = this };
            d.Listen(prefixes);
            d.SetAllowedIps(httpAllowedIps);
            d.Accept();

            this.logger.InfoFormat("Http listen on: " + JsonUtils.stringify(prefixes));
        }

        public void StopListen_Http()
        {
            ref HttpListenerData d = ref this.httpListenerDataForAll;
            if (d != null)
            {
                d.Stop();
                d = null;
            }
        }

        public List<int> busyList { get; } = new List<int>();
        public int busyCount { get; private set; }
        public int lastErrorBusyCount;
        public int AddToBusyList(int msgType)
        {
            this.busyCount++;

            for (int i = 0; i < this.busyList.Count; i++)
            {
                if (this.busyList[i] == -1)
                {
                    this.busyList[i] = msgType;
                    return i;
                }
            }
            this.busyList.Add(msgType);
            return this.busyList.Count - 1;
        }
        public void RemoveFromBusyList(int index)
        {
            this.busyCount--;
            this.busyList[index] = -1;
        }
        public bool BusyListIsEmpty()
        {
            return this.busyCount == 0;
        }

        public ITimer timer_CheckConnections_Loop;

        // 自己的 service-config
        public ServiceConfig serviceConfig;

        public ResGetServiceConfigs current_resGetServiceConfigs;

        public void SaveServiceConfigs(ResGetServiceConfigs res)
        {
            this.current_resGetServiceConfigs = res;
        }

        public List<ServiceConfig> thisServerServiceConfigs
        {
            get
            {
                return this.current_resGetServiceConfigs.serviceConfigs;
            }
        }

        public ServiceData(ServiceTypeAndId serviceTypeAndId, List<ServiceType> connectToServiceIds)
        {
            this.serviceTypeAndId = serviceTypeAndId;

            this.logger = ServerData.instance.log4netCreation.GetLogger(this.serviceTypeAndId.ToString());

            this.connectToServiceTypes.AddRange(connectToServiceIds);
        }

        public virtual void ReloadConfigs(bool all, List<string> files)
        {

        }

        public virtual void GetReloadConfigOptions(List<string> files)
        {

        }
    }
}