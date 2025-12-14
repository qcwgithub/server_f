using Data;

namespace Script
{
    public class AuthService : Service
    {
        public AuthServiceData sd
        {
            get
            {
                return (AuthServiceData)this.data;
            }
        }

        public readonly ConnectToDbService connectToDbService;
        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly ConnectToGatewayService connectToGatewayService;
        public readonly UserIdSnowflakeScript userIdSnowflakeScript;
        public readonly ChannelUuid channelUuid;

        public AuthService(Server server, int serviceId) : base(server, serviceId)
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
            base.AddHandler<AuthService>();

            this.dispatcher.AddHandler(new Auth_Start(this.server, this));
            this.dispatcher.AddHandler(new Auth_Shutdown(this.server, this));
        }
    }
}