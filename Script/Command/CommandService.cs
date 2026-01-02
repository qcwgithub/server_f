using Data;

namespace Script
{
    public partial class CommandService : Service
    {
        public readonly GlobalServiceProxy globalServiceProxy;
        public CommandConnectToOtherService commandConnectToOtherService { get; private set; }

        public CommandService(Server server, int serviceId) : base(server, serviceId)
        {
            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.commandConnectToOtherService = new CommandConnectToOtherService(this);
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
        }

        // public override async Task Detach(ScriptEntry scriptEntry)
        // {
        //     await base.Detach(scriptEntry);
        // }
    }
}