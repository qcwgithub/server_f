using Data;

namespace Script
{
    public partial class GatewayService : Service
    {
        public GatewayServiceData sd
        {
            get
            {
                return (GatewayServiceData)this.data;
            }
        }

        public readonly UserManagerServiceProxy userManagerServiceProxy;
        public readonly UserServiceProxy userServiceProxy;
        protected override TcpListenerScript CreateTcpListenerScriptForC()
        {
            return new TcpListenerScript(this.server, this, false);
        }
        protected override ProtocolClientScriptForS CreateProtocolClientScriptForS()
        {
            return new GatewayProtocolClientScriptForS(this.server, this);
        }
        protected override ProtocolClientScript CreateProtocolClientScriptForC()
        {
            return new GatewayProtocolClientScriptForC(this.server, this);
        }
        public readonly GatewayServiceScript ss;
        public readonly ObjectLocator userLocator;
        public readonly ObjectLocationAssignment userLocationAssignmentScript;

        public GatewayService(Server server, int serviceId) : base(server, serviceId)
        {
            this.AddServiceProxy(this.userManagerServiceProxy = new UserManagerServiceProxy(this));
            this.AddServiceProxy(this.userServiceProxy = new UserServiceProxy(this));

            this.ss = new GatewayServiceScript(this.server, this);
            this.userLocator = ObjectLocator.CreateUserLocator(this.server, this, this.sd.userLocatorData);
            this.userLocationAssignmentScript = ObjectLocationAssignment.CreateUserLocationAssignment(this.server, this, this.sd.userServiceAllocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<GatewayService>();

            this.dispatcher.AddHandler(new Gateway_Action(this.server, this));
            this.dispatcher.AddHandler(new Gateway_ServerKick(this.server, this), true);
            this.dispatcher.AddHandler(new Gateway_UserLogin(this.server, this));
        }
    }
}