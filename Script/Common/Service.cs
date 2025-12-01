using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class BinaryMessagePackerSafe : IMessagePacker
    {
        BinaryMessagePacker _inner;
        public BinaryMessagePackerSafe()
        {
            _inner = new BinaryMessagePacker();
        }

        public bool IsCompeteMessage(byte[] buffer, int offset, int count, out int exactCount)
        {
            return _inner.IsCompeteMessage(buffer, offset, count, out exactCount);
        }

        public UnpackResult Unpack(byte[] buffer, int offset, int count)
        {
            UnpackResult r = _inner.Unpack(buffer, offset, count);
            if (r.totalLength <= 0)
            {
                throw new Exception("r.totalLength <= 0");
            }
            return r;
        }

        public byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse)
        {
            return _inner.Pack(msgTypeOrECode, msg, seq, requireResponse);
        }

        public void ModifySeq(byte[] buffer, int seq)
        {
            _inner.ModifySeq(buffer, seq);
        }
    }

    // Service 提供给 IScript 数据、其他脚本的访问
    public abstract partial class Service
    {
        public readonly Server server;
        public readonly int serviceId;
        public TcpListenerScript tcpListenerScript { get; protected set; }
        public ProtocolClientScriptS baseTcpClientScript { get; private set; }
        public HttpListenerScript httpListenerScript { get; protected set; }
        public WebSocketListenerScript webSocketListenerScript { get; protected set; }

        public MessageDispatcher dispatcher { get; protected set; }
        public IMessagePacker messagePackerBin { get; protected set; }
        public IMessagePacker messagePackerJson { get; protected set; }

        public Service(Server server, int serviceId)
        {
            this.server = server;
            this.serviceId = serviceId;
        }

        protected void AddHandler<S>()
            where S : Service
        {
            this.dispatcher.AddHandler(new OnRemoteWillShutdown<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnGetServiceState<S>().Init((S)this));

            this.dispatcher.AddHandler(new OnConnectComplete<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnConnectorInfo<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnResGetServiceConfigs<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnGetConnectedInfos<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnSocketClose<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnReloadScript<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnReloadConfigs<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnGetReloadConfigOptions<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnGC<S>().Init((S)this));

            this.dispatcher.AddHandler(new CheckConnections_Loop<S>().Init((S)this));
            this.dispatcher.AddHandler(new CheckConnections<S>().Init((S)this));

            this.dispatcher.AddHandler(new OnGetPendingMsgList<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnGetScriptVersion<S>().Init((S)this));

            this.dispatcher.AddHandler(new OnWaitTask<S>().Init((S)this));
            this.dispatcher.AddHandler(new OnViewMongoDumpList<S>().Init((S)this));
        }

        public ServiceData data { get; private set; }
        public log4net.ILog logger => this.data.logger;
        protected abstract ProtocolClientScriptS CreateTcpClientScript();
        public ConnectToSelf connectToSelf { get; private set; }
        public Dictionary<ServiceType, ConnectToOtherService> connectToOtherServiceDict { get; } = new Dictionary<ServiceType, ConnectToOtherService>();
        protected void AddConnectToOtherService(ConnectToOtherService connectToOtherService)
        {
            this.connectToOtherServiceDict.Add(connectToOtherService.to, connectToOtherService);
        }
        public virtual void Attach()
        {
            this.connectToSelf = new ConnectToSelf(this);

            this.tcpListenerScript = new TcpListenerScript().Init(this);
            this.baseTcpClientScript = this.CreateTcpClientScript();
            this.httpListenerScript = new HttpListenerScript().Init(this);
            this.webSocketListenerScript = new WebSocketListenerScript().Init(this);

            this.dispatcher = this.CreateMessageDispatcher(this.server);
            // this.messagePacker = new JsonMessagePackerS().Init(this.server, this);
            this.messagePackerBin = new BinaryMessagePackerSafe();

            this.data = this.server.data.serviceDatas[this.serviceId];

            this.data.tcpClientCallback = this.baseTcpClientScript;
            this.data.tcpListenerCallback = this.tcpListenerScript;
            this.data.httpListenerCallback = this.httpListenerScript;
            this.data.webSocketListenerCallback = this.webSocketListenerScript;
        }
        protected virtual MessageDispatcher CreateMessageDispatcher(Server server)
        {
            return new MessageDispatcher().Init(this);
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

            if (this.data.tcpClientCallback == this.baseTcpClientScript)
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

        // public void ProxyDispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> reply)
        // {
        //     this.data.tcpClientCallback.Dispatch(data, msgType, msg, reply);
        // }

        public virtual void Dispatch(ProtocolClientData data, int seq, MsgType msgType, object msg, Action<ECode, object> reply)
        {
            this.dispatcher.Dispatch(data, msgType, msg, reply);
        }

        public virtual ProtocolClientData GetOrConnectSocket(ServiceType to_serviceType, int to_serviceId, string inIp, int inPort)
        {
            ProtocolClientData socket;
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
            var r = await socket.SendAsync(MsgType._ConfigManager_GetServiceConfigs, msg, null);
            if (r.err != ECode.Success)
            {
                return null;
            }

            var res = r.CastRes<ResGetServiceConfigs>();
            return res;
        }

        protected bool CheckResGetServiceConfigs(ResGetServiceConfigs res, out ServiceConfig myServiceConfig, out string message)
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

            List<ServiceConfig> normals = res.serviceConfigs;

            var self = this.server.data.serverConfig;
            if (normals == null || normals.Count == 0)
            {
                message = "normals == null || normals.Count == 0";
                return false;
            }

            myServiceConfig = normals.Find(x => x.serviceType == this.data.serviceType && x.serviceId == this.data.serviceId);

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

        // override by ConfigManagerService
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

                if (!this.CheckResGetServiceConfigs(res, out ServiceConfig myServiceConfig, out string message))
                {
                    throw new Exception(message);
                }

                this.data.SaveServiceConfigs(res);
                this.data.serviceConfig = myServiceConfig;

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
    }
}
