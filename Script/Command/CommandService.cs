using Data;

namespace Script
{
    public class CommandService : Service
    {
        public ConnectToGlobalService connectToGlobalService { get; private set; }
        public MonitorConnectToSameServerType connectToSameServerType { get; private set; }

        public CommandService(Server server, int serviceId) : base(server, serviceId)
        {
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
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.connectToSameServerType = new MonitorConnectToSameServerType(this);

            this.dispatcher.AddHandler(new Command_Start().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_Shutdown().Init(this.server, this));

            this.dispatcher.AddHandler(new Command_PerformGetPendingMsgList().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformReloadScript().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformSaveProfileToFile().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformShowScriptVersion().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformShutdown().Init(this.server, this));
            this.dispatcher.AddHandler(new Monitor_PerformKick().Init(this.server, this));
            this.dispatcher.AddHandler(new Command_PerformSetPlayerGmFlag().Init(this.server, this));
        }

        // public override async Task Detach(ScriptEntry scriptEntry)
        // {
        //     await base.Detach(scriptEntry);
        // }
    }
}