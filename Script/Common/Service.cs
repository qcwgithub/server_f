using Data;

namespace Script
{
    public abstract partial class Service
    {
        public readonly Server server;
        public readonly int serviceId;
        public readonly TcpListenerScript tcpListenerScript;
        public readonly ProtocolClientScriptS tcpClientScript;
        public readonly HttpListenerScript httpListenerScript;
        public readonly WebSocketListenerScript webSocketListenerScript;
        public readonly MessageDispatcher dispatcher;
        public readonly ConnectToSelf connectToSelf;
        public readonly Dictionary<ServiceType, ConnectToOtherService> connectToOtherServiceDict;

        public Service(Server server, int serviceId)
        {
            this.server = server;
            this.serviceId = serviceId;

            this.connectToSelf = new ConnectToSelf(this);

            this.tcpListenerScript = new TcpListenerScript().Init(this.server, this);
            this.tcpClientScript = new ProtocolClientScriptS().Init(this.server, this); ;
            this.httpListenerScript = new HttpListenerScript().Init(this.server, this);
            this.webSocketListenerScript = new WebSocketListenerScript().Init(this.server, this);

            this.dispatcher = new MessageDispatcher().Init(this.server, this);
            this.connectToOtherServiceDict = new Dictionary<ServiceType, ConnectToOtherService>();
        }

        protected void AddHandler<S>()
            where S : Service
        {
            this.dispatcher.AddHandler(new OnRemoteWillShutdown<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetServiceState<S>().Init(this.server, (S)this));

            this.dispatcher.AddHandler(new OnConnectComplete<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnConnectorInfo<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnResGetServiceConfigs<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetConnectedInfos<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnSocketClose<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnReloadScript<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnReloadConfigs<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetReloadConfigOptions<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGC<S>().Init(this.server, (S)this));

            this.dispatcher.AddHandler(new CheckConnections_Loop<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new CheckConnections<S>().Init(this.server, (S)this));

            this.dispatcher.AddHandler(new OnGetPendingMsgList<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetScriptVersion<S>().Init(this.server, (S)this));

            this.dispatcher.AddHandler(new OnWaitTask<S>().Init(this.server, (S)this));
            this.dispatcher.AddHandler(new OnViewMongoDumpList<S>().Init(this.server, (S)this));
        }

        public ServiceData data { get; private set; }
        public log4net.ILog logger => this.data.logger;
        protected void AddConnectToOtherService(ConnectToOtherService connectToOtherService)
        {
            this.connectToOtherServiceDict.Add(connectToOtherService.to, connectToOtherService);
        }
        public virtual void Attach()
        {
            this.data = this.server.data.serviceDatas[this.serviceId];

            this.data.tcpClientCallback = this.tcpClientScript;
            this.data.tcpListenerCallback = this.tcpListenerScript;
            this.data.httpListenerCallback = this.httpListenerScript;
            this.data.webSocketListenerCallback = this.webSocketListenerScript;
        }

        public bool detaching { get; private set; }
        public bool detached { get; private set; }
        public bool detachingOrDetached => this.detaching || this.detached;
        public void StartDetach()
        {
            Console.WriteLine("**** {0}.Detaching, V{1}", this.data.serviceTypeAndId.ToString(), this.server.GetScriptDllVersion());
            this.detaching = true;
        }

        public virtual async Task Detach()
        {
            while (!this.data.BusyListIsEmpty())
            {
                Console.WriteLine("**** {0}.Detaching, V{1} dispatcher is busy, wait 1 second",
                    this.data.serviceTypeAndId.ToString(), this.server.GetScriptDllVersion());
                await Task.Delay(1000);
            }

            if (this.data.tcpClientCallback == this.tcpClientScript)
            {
                this.data.tcpClientCallback = null;
            }
            if (this.data.tcpListenerCallback == this.tcpListenerScript)
            {
                this.data.tcpListenerCallback = null;
            }
            if (this.data.httpListenerCallback == this.httpListenerScript)
            {
                this.data.httpListenerCallback = null;
            }
            if (this.data.webSocketListenerCallback == this.webSocketListenerScript)
            {
                this.data.webSocketListenerCallback = null;
            }
        }

        public void EndDetach()
        {
            // logger 留着
            // this.logger = null;

            // 这个不需要unload，等着被换掉
            // this.baseData.scriptProxy = null;
            Console.WriteLine("**** {0}.Detached, V{1}", this.data.serviceTypeAndId.ToString(), this.server.GetScriptDllVersion());
            this.data = null;
            this.detached = true;
        }

        public virtual void OnFps(int fps)
        {
            this.dispatcher.OnFps(fps);
        }

        public virtual ProtocolClientData GetOrConnectSocket(ServiceType to_serviceType, int to_serviceId, string inIp, int inPort)
        {
            ProtocolClientData? socket;
            if (!data.otherServiceSockets.TryGetValue(to_serviceId, out socket) || socket.IsClosed())
            {
                socket = new TcpClientData();
                ((TcpClientData)socket).ConnectorInit(this.data, inIp, inPort);
                data.SetOtherServiceSocket(to_serviceType, to_serviceId, socket);
            }

            if (!socket.IsConnected() && !socket.IsConnecting())
            {
                // connect once
                // this.server.logger.Info("call connect to " + serviceId + ", " + this.server.data.getInt("keepServerConnectionsing"));
                socket.Connect();
            }

            return socket;
        }

        protected virtual async Task<ResGetServiceConfigs> RequestServiceConfigs(string why)
        {
            var location = this.server.data.globalServiceLocation;

            ProtocolClientData socket = this.GetOrConnectSocket(ServiceType.Global, location.serviceId, location.inIp, location.inPort);
            if (socket == null || !socket.IsConnected())
            {
                return null;
            }

            var msg = new MsgGetServiceConfigs();
            msg.fromServiceType = this.data.serviceType;
            msg.fromServiceId = this.data.serviceId;
            msg.why = why;
            var r = await socket.Request<MsgGetServiceConfigs, ResGetServiceConfigs>(MsgType._Global_GetServiceConfigs, msg);
            if (r.e != ECode.Success)
            {
                return null;
            }

            return r.res;
        }

        protected bool CheckResGetServiceConfigs(ResGetServiceConfigs res, out ServiceConfig? myServiceConfig, out string message)
        {
            myServiceConfig = null;
            message = string.Empty;

            string myPurpose = this.server.data.serverConfig.purpose;
            if (myPurpose != res.purpose)
            {
                message = $"myPurpose('{myPurpose}') != res.purpose('{res.purpose}')";
                return false;
            }

            Version myVersion = this.server.GetScriptDllVersion();
            if (myVersion.Major != res.majorVersion ||
                myVersion.Minor != res.minorVerson)
            {
                message = $"myVersion({myVersion.Major}.{myVersion.Minor}) != res.version({res.majorVersion}.{res.minorVerson})";
                return false;
            }

            List<ServiceConfig> allServiceConfigs = res.allServiceConfigs;

            var self = this.server.data.serverConfig;
            if (allServiceConfigs == null || allServiceConfigs.Count == 0)
            {
                message = "allServiceConfigs == null || allServiceConfigs.Count == 0";
                return false;
            }

            myServiceConfig = allServiceConfigs.Find(x => x.serviceType == this.data.serviceType && x.serviceId == this.data.serviceId);

            if (myServiceConfig == null)
            {
                message = "myServiceConfig == null";
                return false;
            }

            if (myServiceConfig.serviceType.IsCommand())
            {
                // command 不需要 ip 正确
            }
            else
            {
                if (myServiceConfig.inIp != "localhost" &&
                    myServiceConfig.inIp != "127.0.0.1" &&
                    myServiceConfig.inIp != Data.Program.s_inIp)
                {
                    message = $"myServiceConfig.inIp({myServiceConfig.inIp}) != Data.Program.s_inIp({Data.Program.s_inIp})";
                    return false;
                }
            }

            return true;
        }

        // override by GlobalService
        public virtual async Task<ECode> InitServiceConfigsUntilSuccess()
        {
            int counter = 0;

            while (true)
            {
                if (this.data.state >= ServiceState.ShuttingDown)
                {
                    return ECode.ServiceIsShuttingDown;
                }

                counter++;
                if (counter == 2)
                {
                    this.logger.Info("Wait GetServiceConfigs...");
                }

                ResGetServiceConfigs res = await this.RequestServiceConfigs("init");
                if (res == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                if (!this.CheckResGetServiceConfigs(res, out ServiceConfig? myServiceConfig, out string message))
                {
                    throw new Exception(message);
                }

                this.data.SaveServiceConfigs(res);
                this.data.serviceConfig = myServiceConfig!;

                if (counter >= 2)
                {
                    this.logger.Info("Wait GetServiceConfigs...Done");
                }

                break;
            }

            return ECode.Success;
        }

        public ConnectorInfo CreateConnectorInfo()
        {
            var connectorInfo = new ConnectorInfo();
            connectorInfo.serviceType = this.data.serviceType;
            connectorInfo.serviceId = this.data.serviceId;
            connectorInfo.serviceState = this.data.state;
            return connectorInfo;
        }

        public async Task<ECode> WaitServiceConnectedAndStarted(ConnectToOtherService connectToOtherService, MsgType msgType)
        {
            int counter = 0;

            while (true)
            {
                if (this.data.state >= ServiceState.ShuttingDown)
                {
                    return ECode.ServiceIsShuttingDown;
                }

                counter++;
                if (counter == 2)
                {
                    this.logger.InfoFormat("{0} Wait connect to {1}...", msgType, connectToOtherService.to);
                }

                var msg = new MsgGetServiceState();
                var r = await connectToOtherService.Send<MsgGetServiceState, ResGetServiceState>(MsgType._GetServiceState, msg);
                if (r.e != ECode.Success)
                {
                    await Task.Delay(100);
                    continue;
                }

                if (r.res.serviceState != ServiceState.Started)
                {
                    await Task.Delay(100);
                    continue;
                }

                if (counter >= 2)
                {
                    this.logger.InfoFormat("{0} Wait connect to {1}...Done", msgType, connectToOtherService.to);
                }
                break;
            }

            return ECode.Success;
        }

        public bool IsShuttingDown()
        {
            return this.data.state >= ServiceState.ShuttingDown;
        }

        public void SetState(ServiceState s)
        {
            this.data.state = s;
            this.logger.Info(s);
        }
    }
}
