using Data;

namespace Script
{
    public class CommandService : Service
    {
        public ConnectToGlobalService connectToGlobalService { get; private set; }
        public MonitorConnectToSameServerType connectToSameServerType { get; private set; }

        public CommandService(Server server, int serviceId) : base(server, serviceId)
        {
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.connectToSameServerType = new MonitorConnectToSameServerType(this);
        }

        public CommandServiceData commandServiceData
        {
            get
            {
                return (CommandServiceData)this.data;
            }
        }

        public override void Attach()
        {
            base.Attach();

            base.AddHandler<CommandService>();

            this.dispatcher.AddHandler(new Command_Start(this.server, this));
            this.dispatcher.AddHandler(new Command_Shutdown(this.server, this));

            this.dispatcher.AddHandler(new Command_PerformGetPendingMsgList(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformReloadScript(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformSaveProfileToFile(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformShowScriptVersion(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformShutdown(this.server, this));
            this.dispatcher.AddHandler(new Monitor_PerformKick(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformSetPlayerGmFlag(this.server, this));
        }

        // public override async Task Detach(ScriptEntry scriptEntry)
        // {
        //     await base.Detach(scriptEntry);
        // }
    }
}