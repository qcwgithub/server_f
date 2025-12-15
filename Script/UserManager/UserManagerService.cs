using Data;

namespace Script
{
    public class UserManagerService : Service
    {
        public UserManagerServiceData sd
        {
            get
            {
                return (UserManagerServiceData)this.data;
            }
        }

        public readonly ConnectToDbService connectToDbService;
        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly ConnectToGatewayService connectToGatewayService;
        public readonly UserIdSnowflakeScript userIdSnowflakeScript;
        public readonly ChannelUuid channelUuid;

        public UserManagerService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddConnectToOtherService(this.connectToDbService = new ConnectToDbService(this));
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.AddConnectToOtherService(this.connectToGatewayService = new ConnectToGatewayService(this));

            this.userIdSnowflakeScript = new UserIdSnowflakeScript(this.server, this);
            this.channelUuid = new ChannelUuid(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserManagerService>();

            this.dispatcher.AddHandler(new UserManager_Start(this.server, this));
            this.dispatcher.AddHandler(new UserManager_Shutdown(this.server, this));
        }
    }
}