using Data;

namespace Script
{
    public class UserService : Service
    {
        public UserServiceData sd
        {
            get
            {
                return (UserServiceData)this.data;
            }
        }

        //
        public readonly ConnectToDbService connectToDbService;
        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly ConnectToGatewayService connectToGatewayService;

        public readonly UserServiceScript ss;

        public UserService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddConnectToOtherService(this.connectToDbService = new ConnectToDbService(this));
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.AddConnectToOtherService(this.connectToGatewayService = new ConnectToGatewayService(this));

            this.ss = new UserServiceScript().Init(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
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
            this.dispatcher.AddHandler(new User_SetGmFlag().Init(this.server, this));
        }

        public override async Task Detach()
        {
            await base.Detach();
        }
    }
}