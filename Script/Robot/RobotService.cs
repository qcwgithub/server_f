using Data;

namespace Script
{
    public partial class RobotService : Service
    {
        public RobotServiceData sd
        {
            get
            {
                return (RobotServiceData)this.data;
            }
        }

        public readonly ProtocolClientScriptRobotClient protocolClientScriptRobotClient;
        public RobotService(Server server, int serviceId) : base(server, serviceId)
        {
            this.protocolClientScriptRobotClient = new ProtocolClientScriptRobotClient(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<RobotService>();

            this.sd.protocolClientCallbackProviderRobotClient.protocolClientCallbackRobotClient = this.protocolClientScriptRobotClient;
        }

        public override async Task Detach()
        {
            await base.Detach();
        }
    }
}