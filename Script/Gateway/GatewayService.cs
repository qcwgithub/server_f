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
        public readonly UserServiceManager userServiceManager;
        protected override TcpListenerScript CreateTcpListenerScriptForC()
        {
            return new GatewayTcpListenerScriptForC(this.server, this);
        }
        protected override ProtocolClientScript CreateProtocolClientScriptForC()
        {
            return new GatewayProtocolClientScriptForC(this.server, this);
        }
        public GatewayService(Server server, int serviceId) : base(server, serviceId)
        {
            this.connectToUserManagerService = new ConnectToUserManagerService(this);
            this.userServiceManager = new UserServiceManager(this.server, this);
        }
        
        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();

            this.dispatcher.AddHandler(new Gateway_Start(this.server, this));
            this.dispatcher.AddHandler(new Gateway_Shutdown(this.server, this));
        }
    }
}