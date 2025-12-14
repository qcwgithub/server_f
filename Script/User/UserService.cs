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

            this.ss = new UserServiceScript(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserService>();

            this.dispatcher.AddHandler(new User_Start(this.server, this));
            this.dispatcher.AddHandler(new User_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new User_OnReloadConfigs(this.server, this), true);
            this.dispatcher.AddHandler(new User_OnConnectComplete(this.server, this), true);
            this.dispatcher.AddHandler(new User_Action(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserInfoToFile(this.server, this));
            this.dispatcher.AddHandler(new User_DestroyUser(this.server, this));
            this.dispatcher.AddHandler(new User_OnConnectionClose(this.server, this), true);
            this.dispatcher.AddHandler(new User_SaveUser(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserImmediately(this.server, this));
            this.dispatcher.AddHandler(new User_SetGmFlag(this.server, this));
        }

        public override async Task Detach()
        {
            await base.Detach();
        }
    }
}