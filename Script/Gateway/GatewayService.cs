using Data;

namespace Script
{
    public class GatewayService : Service
    {
        public GatewayServiceData sd
        {
            get
            {
                return (GatewayServiceData)this.data;
            }
        }

        public readonly ConnectToUserManagerService connectToUserManagerService;
        public readonly ConnectToUserService connectToUserService;
        public readonly UserServiceManager userServiceManager;
        protected override TcpListenerScript CreateTcpListenerScriptForC()
        {
            return new GatewayTcpListenerScriptForC(this.server, this);
        }
        protected override ProtocolClientScript CreateProtocolClientScriptForC()
        {
            return new GatewayProtocolClientScriptForC(this.server, this);
        }
        public readonly GatewayServiceScript ss;
        public readonly UserServiceAllocator<GatewayService> userServiceAllocator;

        public GatewayService(Server server, int serviceId) : base(server, serviceId)
        {
            this.connectToUserManagerService = new ConnectToUserManagerService(this);
            this.connectToUserService = new ConnectToUserService(this);
            this.userServiceManager = new UserServiceManager(this.server, this);

            this.ss = new GatewayServiceScript(this.server, this);
            this.userServiceAllocator = new UserServiceAllocator<GatewayService>(this.server, this, this.sd.userServiceAllocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();

            this.dispatcher.AddHandler(new Gateway_Start(this.server, this));
            this.dispatcher.AddHandler(new Gateway_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new Gateway_UserLogin(this.server, this));
            this.dispatcher.AddHandler(new Gateway_OnConnectionClose(this.server, this), true);
        }
    }
}