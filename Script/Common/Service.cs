using Data;

namespace Script
{
    public abstract partial class Service
    {
        public readonly Server server;
        public readonly int serviceId;
        public readonly ServiceData data;

        public readonly TcpListenerScript tcpListenerScriptForS;
        public readonly TcpListenerScript tcpListenerScriptForC;
        protected virtual TcpListenerScript CreateTcpListenerScriptForC()
        {
            return null;
        }

        public readonly ConnectionCallbackScriptForS connectionCallbackScriptForS;
        protected virtual ConnectionCallbackScriptForS CreateConnectionCallbackScriptForS()
        {
            return new ConnectionCallbackScriptForS(this.server, this);
        }
        public readonly ConnectionCallbackScript connectionCallbackScriptForC;
        protected virtual ConnectionCallbackScript CreateConnectionCallbackScriptForC()
        {
            return null;
        }

        public readonly HttpListenerScript httpListenerScript;
        public readonly WebSocketListenerScript webSocketListenerScript;
        public readonly ForwardingScript forwardingScript;

        public readonly MessageDispatcher dispatcher;
        protected virtual MessageDispatcher CreateMessageDispatcher()
        {
            return new MessageDispatcher(this.server, this);
        }

        public readonly Dictionary<ServiceType, ServiceProxy> serviceProxyDict;
        public ServiceProxy GetServiceProxy(ServiceType serviceType)
        {
            return this.serviceProxyDict[serviceType];
        }

        public readonly LockManuallyScript lockManuallyScript;

        public Service(Server server, int serviceId)
        {
            this.server = server;
            this.serviceId = serviceId;
            this.data = this.server.data.serviceDatas[this.serviceId];

            this.tcpListenerScriptForS = new TcpListenerScript(this.server, this, false);
            this.tcpListenerScriptForC = this.CreateTcpListenerScriptForC();

            this.connectionCallbackScriptForS = this.CreateConnectionCallbackScriptForS();
            this.connectionCallbackScriptForC = this.CreateConnectionCallbackScriptForC();

            this.httpListenerScript = new HttpListenerScript(this.server, this);
            this.webSocketListenerScript = new WebSocketListenerScript(this.server, this);
            this.forwardingScript = new ForwardingScript(this.server, this);

            this.dispatcher = this.CreateMessageDispatcher();

            this.serviceProxyDict = new();
            this.lockManuallyScript = new LockManuallyScript(this.server, this);
        }

        protected void AddHandler<S>()
            where S : Service
        {
            this.dispatcher.AddHandler(new OnRemoteWillShutdown<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetServiceState<S>(this.server, (S)this));

            this.dispatcher.AddHandler(new OnConnectorInfo<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnResGetServiceConfigs<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetConnectedInfos<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnReloadScript<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnReloadConfigs<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnTimer<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetReloadConfigOptions<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGC<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnShutdown<S>(this.server, (S)this));

            this.dispatcher.AddHandler(new OnGetPendingMsgList<S>(this.server, (S)this));
            this.dispatcher.AddHandler(new OnGetScriptVersion<S>(this.server, (S)this));

            this.dispatcher.AddHandler(new OnViewMongoDumpList<S>(this.server, (S)this));
        }

        public log4net.ILog logger => this.data.logger;
        protected void AddServiceProxy(ServiceProxy serviceProxy)
        {
            this.serviceProxyDict.Add(serviceProxy.to, serviceProxy);
        }
        public virtual void Attach()
        {
            this.data.connectionCallbackForS = this.connectionCallbackScriptForS;
            this.data.connectionCallbackForC = this.connectionCallbackScriptForC;

            this.data.tcpListenerCallbackForS = this.tcpListenerScriptForS;
            this.data.tcpListenerCallbackForC = this.tcpListenerScriptForC;

            this.data.httpListenerCallback = this.httpListenerScript;
            this.data.webSocketListenerCallback = this.webSocketListenerScript;

            this.data.sendClientMessageThroughGateway = this.forwardingScript;
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

            if (this.data.connectionCallbackForS == this.connectionCallbackScriptForS)
            {
                this.data.connectionCallbackForS = null;
            }
            if (this.data.connectionCallbackForC == this.connectionCallbackScriptForC)
            {
                this.data.connectionCallbackForC = null;
            }

            if (this.data.tcpListenerCallbackForS == this.tcpListenerScriptForS)
            {
                this.data.tcpListenerCallbackForS = null;
            }
            if (this.data.tcpListenerCallbackForC == this.tcpListenerScriptForC)
            {
                this.data.tcpListenerCallbackForC = null;
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
            // this.data = null;
            this.detached = true;
        }

        public virtual void OnFps(int fps)
        {
            this.dispatcher.OnFps(fps);
        }

        public virtual IServiceConnection GetServiceConnectionOrConnect(ServiceType to_serviceType, int to_serviceId, string inIp, int inPort)
        {
            IServiceConnection? connection;
            if (!data.otherServiceConnections.TryGetValue(to_serviceId, out connection) || connection.IsClosed())
            {
                // if (this.server.data.serviceTypeAndIds.Exists(tai => tai.serviceType == to_serviceType && tai.serviceId == to_serviceId))
                // {
                //     connection = new InProcessServiceConnection(to_serviceType, to_serviceId);
                //     data.SaveOtherServiceConnection(connection);
                // }
                // else
                {
                    connection = new SocketServiceConnection(this.data, inIp, inPort, to_serviceType, to_serviceId);
                    data.SaveOtherServiceConnection(connection);

                    if (!connection.IsConnected() && !connection.IsConnecting())
                    {
                        // connect once
                        // this.server.logger.Info("call connect to " + serviceId + ", " + this.server.data.getInt("keepServerConnectionsing"));
                        connection.Connect();
                    }
                }
            }

            return connection;
        }

        protected virtual async Task<ResGetServiceConfigs?> RequestServiceConfigs(string why)
        {
            var location = this.server.data.globalServiceLocation;

            IServiceConnection connection = this.GetServiceConnectionOrConnect(ServiceType.Global, location.serviceId, location.inIp, location.inPort);
            if (connection == null || !connection.IsConnected())
            {
                return null;
            }

            var msg = new MsgGetServiceConfigs();
            msg.fromServiceType = this.data.serviceType;
            msg.fromServiceId = this.data.serviceId;
            msg.why = why;

            var r = await ((GlobalServiceProxy)this.GetServiceProxy(ServiceType.Global)).GetServiceConfigs(msg);
            if (r.e != ECode.Success)
            {
                return null;
            }

            return r.CastRes<ResGetServiceConfigs>();
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

            if (myServiceConfig.inIp != "localhost" &&
                myServiceConfig.inIp != "127.0.0.1" &&
                myServiceConfig.inIp != Data.Program.s_inIp)
            {
                message = $"myServiceConfig.inIp({myServiceConfig.inIp}) != Data.Program.s_inIp({Data.Program.s_inIp})";
                return false;
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

                ResGetServiceConfigs? res = await this.RequestServiceConfigs("init");
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

        public async Task<ECode> WaitServiceConnectedAndStarted(ServiceProxy serviceProxy)
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
                    this.logger.InfoFormat("Wait connect to {0}...", serviceProxy.to);
                }

                var msg = new MsgGetServiceState();
                MyResponse r;

                int sid = this.data.GetFirstConnected(serviceProxy.to);
                if (sid == 0)
                {
                    r = new MyResponse(ECode.NotConnected);
                }
                else
                {
                    r = await serviceProxy.GetServiceState(sid, msg);
                }
                if (r.e != ECode.Success)
                {
                    await Task.Delay(100);
                    continue;
                }

                var res = r.CastRes<ResGetServiceState>();
                if (res.serviceState != ServiceState.Started)
                {
                    await Task.Delay(100);
                    continue;
                }

                if (counter >= 2)
                {
                    this.logger.InfoFormat("Wait connect to {0}...Done", serviceProxy.to);
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


        protected async Task OnErrorExit(ECode e)
        {
            Console.WriteLine("Error: {0}, Process is exiting", e);
            this.logger.FatalFormat("Error: {0}, Process is exiting", e);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }

        /*
        protected async Task OnErrorExit(string message)
        {
            this.service.logger.ErrorFormat("{0} Error: {1}, Process is exiting", this.msgType, message);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }
        */

        protected async Task OnErrorExit(Exception ex)
        {
            this.logger.Fatal(string.Format("Exception, Process is exiting"), ex);
            await Task.Delay(1000);
            System.Environment.Exit(1);
        }
    }
}
