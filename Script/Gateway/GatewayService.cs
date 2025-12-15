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

        public readonly ConnectToAuthService connectToAuthService;
        public readonly UserServiceManager userServiceManager;
        protected override ProtocolClientScript CreateProtocolClientScriptForC()
        {
            return new GatewayProtocolClientScriptForC(this.server, this);
        }
        public GatewayService(Server server, int serviceId) : base(server, serviceId)
        {
            this.connectToAuthService = new ConnectToAuthService(this);
            this.userServiceManager = new UserServiceManager(this.server, this);
        }
        
        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();

            this.dispatcher.AddHandler(new Gateway_Start(this.server, this));
            this.dispatcher.AddHandler(new Gateway_Shutdown(this.server, this));
        }

        public override async Task Detach()
        {
            if (this.data.protocolClientCallbackForC == this.protocolClientScriptForC)
            {
                this.data.protocolClientCallbackForC = null;
            }

            await base.Detach();
        }
    }
}