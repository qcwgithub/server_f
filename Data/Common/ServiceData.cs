using log4net;

namespace Data
{
    public abstract class ServiceData :
        IConnectionCallbackProvider,
        ITcpListenerCallbackProvider,
        IHttpListenerCallbackProvider,
        IWebSocketListenerCallbackProvider,
        ISendClientMessageThroughGatewayProvider
    {
        public readonly ServerData serverData;
        public bool replyServerTime = true;

        public ServiceType serviceType => this.serviceTypeAndId.serviceType;
        public int serviceId => this.serviceTypeAndId.serviceId;
        public readonly ServiceTypeAndId serviceTypeAndId;
        public ServiceState state = ServiceState.Initing;

        public ServiceData(ServerData serverData, ServiceTypeAndId serviceTypeAndId, List<ServiceType> connectToServiceIds)
        {
            this.serverData = serverData;
            this.serviceTypeAndId = serviceTypeAndId;

            this.logger = ServerData.instance.log4netCreation.GetLogger(this.serviceTypeAndId.ToString());

            this.connectToServiceTypes.AddRange(connectToServiceIds);
        }

        public int errorCount = 0;
        public int defaultDateTime = 0;
        public int msgSeq = 1;

        public Random random = new Random();
        public readonly ILog logger;

        // tcp listener
        public ITcpListenerCallback? tcpListenerCallbackForS;
        public ITcpListenerCallback? tcpListenerCallbackForC;
        public ITcpListenerCallback? GetTcpListenerCallback(TcpListenerData tcpListenerData)
        {
            return tcpListenerData.isForClient ? this.tcpListenerCallbackForC : this.tcpListenerCallbackForS;
        }

        // tcp client callback
        public IConnectionCallback? connectionCallbackForS;
        public IConnectionCallback? connectionCallbackForC;
        public IConnectionCallback GetConnectionCallback(bool forClient)
        {
            return forClient ? this.connectionCallbackForC! : this.connectionCallbackForS!;
        }

        public TcpListenerData? tcpListenerForServer;
        public TcpListenerData? tcpListenerForClient;

        // http listener
        public IHttpListenerCallback? httpListenerCallback;
        public IHttpListenerCallback? GetHttpListenerCallback() => this.httpListenerCallback;
        public HttpListenerData? httpListenerDataForAll;

        public IWebSocketListenerCallback? webSocketListenerCallback;
        public IWebSocketListenerCallback? GetWebSocketListenerCallback() => this.webSocketListenerCallback;
        public WebSocketListenerData? webSocketListenerDataForServer;
        public WebSocketListenerData? webSocketListenerDataForClient;

        public ISendClientMessageThroughGateway? sendClientMessageThroughGateway;
        public ISendClientMessageThroughGateway? Get_S_to_G() => this.sendClientMessageThroughGateway;

        // 需要连接到哪些服务器
        public readonly List<ServiceType> connectToServiceTypes = new List<ServiceType>();

        // 只存 Normal -> Normal 和 Group -> Group
        public Dictionary<int, IServiceConnection> otherServiceConnections = new Dictionary<int, IServiceConnection>();
        public List<IServiceConnection>[] otherServiceConnections2 = new List<IServiceConnection>[(int)ServiceType.Count];
        public void SaveOtherServiceConnection(IServiceConnection connection)
        {
            MyDebug.Assert(connection.knownWho);
            ServiceType serviceType = connection.serviceType;
            int serviceId = connection.serviceId;

            {
                if (this.otherServiceConnections.TryGetValue(serviceId, out IServiceConnection? old))
                {
                    if (old.IsConnected() || old.IsConnecting())
                    {
                        this.logger.Error($"SetOtherServiceSocket tai {serviceType}{serviceId} old.IsConnected() || old.IsConnecting()");
                    }
                }
            }

            this.otherServiceConnections[serviceId] = connection;

            var list = this.otherServiceConnections2[(int)serviceType];
            if (list == null)
            {
                list = this.otherServiceConnections2[(int)serviceType] = new List<IServiceConnection>();
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

                    if (old.serviceId != serviceId)
                    {
                        continue;
                    }

                    list.RemoveAt(i);
                    i--;
                }
            }

            if (list.IndexOf(connection) < 0)
            {
                list.Add(connection);
            }
        }
        public IServiceConnection? GetOtherServiceConnection(int serviceId)
        {
            return this.otherServiceConnections.TryGetValue(serviceId, out IServiceConnection? connection) ? connection : null;
        }

        // 有没有被动连接还活着
        public virtual int GetPassivelyConnectionsCount()
        {
            int count = 0;

            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                if (this.connectToServiceTypes.Contains(serviceType))
                {
                    continue;
                }

                List<IServiceConnection> connections = this.otherServiceConnections2[(int)serviceType];
                if (connections == null || connections.Count == 0)
                {
                    continue;
                }

                foreach (IServiceConnection connection in connections)
                {
                    if (connection.IsConnected())
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        // 关闭所有主动发起的连接
        public virtual async Task CloseProactiveConnections()
        {
            for (ServiceType serviceType = 0; serviceType < ServiceType.Count; serviceType++)
            {
                if (!this.connectToServiceTypes.Contains(serviceType))
                {
                    continue;
                }

                List<IServiceConnection> connections = this.otherServiceConnections2[(int)serviceType];
                if (connections == null || connections.Count == 0)
                {
                    continue;
                }

                int total = 0;
                int finish = 0;

                var tcs = new TaskCompletionSource();
                foreach (IServiceConnection connection in connections)
                {
                    if (connection.IsConnected())
                    {
                        total++;
                        connection.Send(MsgType._Service_RemoteWillShutdown, new MsgRemoteWillShutdown(), (e, segment) =>
                        {
                            finish++;
                            if (finish >= total)
                            {
                                tcs.SetResult();
                            }
                        },
                        pTimeoutS: 5);
                    }
                }
                if (total == 0)
                {
                    tcs.SetResult();
                }
                await tcs.Task;

                List<int> serviceIds = new List<int>();
                foreach (IServiceConnection connection in connections)
                {
                    serviceIds.Add(connection.serviceId);
                }

                foreach (IServiceConnection connection in connections)
                {
                    connection.Close("manual close");
                }
                connections.Clear();

                foreach (int serviceId in serviceIds)
                {
                    this.otherServiceConnections.Remove(serviceId);
                }
            }
        }

        public virtual async Task CloseAllConnections()
        {
            // 主动关闭的也提前告知
            // -----------------------------------------------------------
            int total = 0;
            int finish = 0;

            var tcs = new TaskCompletionSource();
            foreach (var kv in this.otherServiceConnections)
            {
                IServiceConnection connection = kv.Value;
                if (connection.IsConnected())
                {
                    total++;
                    connection.Send(MsgType._Service_RemoteWillShutdown, new MsgRemoteWillShutdown(), (e, segment) =>
                    {
                        finish++;
                        if (finish >= total)
                        {
                            tcs.SetResult();
                        }
                    }, pTimeoutS: 5);
                }
            }
            if (total == 0)
            {
                tcs.SetResult();
            }
            await tcs.Task;

            // -----------------------------------------------------------

            foreach (var kv in this.otherServiceConnections)
            {
                IServiceConnection connection = kv.Value;
                connection.Close("manual close");
            }
            this.otherServiceConnections.Clear();

            foreach (var list in this.otherServiceConnections2)
            {
                if (list != null)
                {
                    list.Clear();
                }
            }
        }

        public int GetFirstConnected(ServiceType to)
        {
            List<IServiceConnection> connections = this.otherServiceConnections2[(int)to];
            if (connections == null || connections.Count == 0)
            {
                return 0;
            }

            foreach (IServiceConnection connection in connections)
            {
                if (connection.IsConnected())
                {
                    return connection.serviceId;
                }
            }

            return 0;
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
        public string? GetString(string key)
        {
            string? value;
            return this.stringMap.TryGetValue(key, out value) ? value : null;
        }
        public void SetString(string key, string value)
        {
            this.stringMap[key] = value;
        }

        public void ListenForServer_Tcp()
        {
            ref TcpListenerData? d = ref this.tcpListenerForServer;
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
            ref TcpListenerData? d = ref this.tcpListenerForServer;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void ListenForClient_Tcp(int outPort)
        {
            ref TcpListenerData? d = ref this.tcpListenerForClient;

            MyDebug.Assert(d == null);
            d = new TcpListenerData() { isForClient = true, callbackProvider = this };
            this.logger.InfoFormat("ListenForClient_Tcp outPort {0}", outPort);
            d.Listen(outPort);
            d.StartAccept();
        }
        public void StopListenForClient_Tcp()
        {
            ref TcpListenerData? d = ref this.tcpListenerForClient;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void ListenForClient_Ws(string[] clientListener_wsPrefixes)
        {
            ref WebSocketListenerData? d = ref this.webSocketListenerDataForClient;

            MyDebug.Assert(d == null);

            d = new WebSocketListenerData() { isForClient = true, callbackProvider = this };
            this.logger.InfoFormat("WebSocket listen for client on: " + JsonUtils.stringify(clientListener_wsPrefixes));
            d.Listen(clientListener_wsPrefixes);
            d.StartAccept();

        }
        public void StopListenForClient_Ws()
        {
            ref WebSocketListenerData? d = ref this.webSocketListenerDataForClient;
            if (d != null)
            {
                d.Close();
                d = null;
            }
        }

        public void Listen_Http(string[] prefixes, List<string> httpAllowedIps)
        {
            ref HttpListenerData? d = ref this.httpListenerDataForAll;

            MyDebug.Assert(d == null);

            d = new HttpListenerData() { callbackProvider = this };
            d.Listen(prefixes);
            d.SetAllowedIps(httpAllowedIps);
            d.Accept();

            this.logger.InfoFormat("Http listen on: " + JsonUtils.stringify(prefixes));
        }

        public void StopListen_Http()
        {
            ref HttpListenerData? d = ref this.httpListenerDataForAll;
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

        public ITimer timer_CheckConnections;

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
                return this.current_resGetServiceConfigs.allServiceConfigs;
            }
        }

        public virtual void ReloadConfigs(bool all, List<string> files)
        {

        }

        public virtual void GetReloadConfigOptions(List<string> files)
        {

        }
    }
}