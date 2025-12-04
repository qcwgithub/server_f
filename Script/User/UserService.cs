using Data;

namespace Script
{
    public class UserService : Service, IConnectToDatabaseService
    {
        //
        public ConnectToDatabaseService connectToDatabaseService { get; private set; }
        public ConnectToGlobalService connectToGlobalService { get; private set; }

        public UserServiceData sd
        {
            get
            {
                return (UserServiceData)this.data;
            }
        }
        public UserServiceScript ss;

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
            this.dispatcher.AddHandler(new User_SetGmFlag().Init(this.server, this));

            this.ss = new UserServiceScript().Init(this.server, this);
        }

        public override async Task Detach()
        {
            await base.Detach();
        }
    }
}