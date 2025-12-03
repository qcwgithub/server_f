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

            this.dispatcher.AddHandler(new User_Start().Init(this.server, this));
            this.dispatcher.AddHandler(new User_Shutdown().Init(this.server, this));
            this.dispatcher.AddHandler(new User_OnReloadConfigs().Init(this.server, this), true);
            this.dispatcher.AddHandler(new User_OnConnectComplete().Init(this.server, this), true);
            this.dispatcher.AddHandler(new User_Action().Init(this.server, this));
            this.dispatcher.AddHandler(new User_SaveProfileToFile().Init(this.server, this));
            this.dispatcher.AddHandler(new User_DestroyUser().Init(this.server, this));
            this.dispatcher.AddHandler(new User_OnSocketClose().Init(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUser().Init(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserImmediately().Init(this.server, this));

            this.usScript = new UserServiceScript().Init(this.server, this);
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
    }
}