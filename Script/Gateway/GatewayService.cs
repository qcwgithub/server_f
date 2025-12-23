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
        protected override TcpListenerScript CreateTcpListenerScriptForC()
        {
            return new TcpListenerScript(this.server, this, false);
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

            this.ss = new GatewayServiceScript(this.server, this);
            this.userServiceAllocator = new UserServiceAllocator<GatewayService>(this.server, this, this.sd.userServiceAllocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();

            this.dispatcher.AddHandler(new Gateway_Action(this.server, this));
            this.dispatcher.AddHandler(new Gateway_DestroyUser(this.server, this));
            this.dispatcher.AddHandler(new Gateway_OnConnectionClose(this.server, this), true);
            this.dispatcher.AddHandler(new Gateway_ServerKick(this.server, this), true);
            this.dispatcher.AddHandler(new Gateway_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new Gateway_Start(this.server, this));
            this.dispatcher.AddHandler(new Gateway_UserLogin(this.server, this));
        }
    }
}