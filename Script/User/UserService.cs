using Data;

namespace Script
{
    public class UserService : Service, IConnectToDatabaseService
    {
        //
        public ConnectToDatabaseService connectToDatabaseService { get; private set; }
        public ConnectToGlobalService connectToGlobalService { get; private set; }

        public UserServiceData usData
        {
            get
            {
                return (UserServiceData)this.data;
            }
        }
        public UserServiceScript usScript;

        public UserService(Server server, int serviceId) : base(server, serviceId)
        {
        }

        public override void Attach()
        {
            base.Attach();

            //
            this.AddConnectToOtherService(this.connectToDatabaseService = new ConnectToDatabaseService(this));
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));

            base.AddHandler<UserService>();

            // 覆盖 OnConnectComplete
            this.dispatcher.AddHandler(new User_OnConnectComplete().Init(this), true);

            this.usScript = new UserServiceScript().Init(this);
        }

        public override async Task Detach()
        {
            await base.Detach();
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

                var r = await connectToOtherService.SendAsync(MsgType._GetServiceState, null);
                if (r.err != ECode.Success)
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

        public async Task SendPSInfoToAAA(bool all, ProtocolClientData socket)
        {
            var serviceConfig = this.usData.serviceConfig;

            var psInfo = new PSInfo();
            psInfo.serviceId = this.serviceId;
            psInfo.playerCount = this.usData.playerDict.Count;
            psInfo.allowNewPlayer = this.usData.allowNewPlayer;

            //
            psInfo.outIp = serviceConfig.outIp;
            psInfo.outPort = serviceConfig.outPort;

            //
            psInfo.ws_outIp = serviceConfig.ws_outIp;
            psInfo.ws_outPort = serviceConfig.ws_outPort;

            var msgA = new MsgPSInfo();
            msgA.psInfo = psInfo;

            if (all)
            {
                await this.connectToStatelessService.SendToAllAsync(MsgType._AAA_PSInfo, msgA);
            }
            else
            {
                await socket.SendAsync(MsgType._AAA_PSInfo, msgA, pTimeoutS: null);
            }
        }
    }
}