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
        public readonly ConnectFromUserService connectFromUserService;
        public readonly UserIdSnowflakeScript userIdSnowflakeScript;
        public readonly ChannelUuid channelUuid;
        public readonly UserManagerServiceScript ss;
        public readonly ObjectLocator userLocator;
        public readonly ObjectLocationAssignment userLocationAssignmentScript;

        public UserManagerService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddConnectToOtherService(this.connectToDbService = new ConnectToDbService(this));
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.AddConnectToOtherService(this.connectToGatewayService = new ConnectToGatewayService(this));
            this.AddConnectToOtherService(this.connectFromUserService = new ConnectFromUserService(this));

            this.userIdSnowflakeScript = new UserIdSnowflakeScript(this.server, this);
            this.channelUuid = new ChannelUuid(this.server, this);
            this.ss = new UserManagerServiceScript(this.server, this);
            this.userLocator = ObjectLocator.CreateUserLocator(this.server, this, this.sd.userLocatorData);
            this.userLocationAssignmentScript = ObjectLocationAssignment.CreateUserLocationAssignment(this.server, this, this.sd.userServiceAllocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserManagerService>();

            this.dispatcher.AddHandler(new UserManager_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new UserManager_Start(this.server, this));
            this.dispatcher.AddHandler(new UserManager_UserLogin(this.server, this));
        }
    }
}