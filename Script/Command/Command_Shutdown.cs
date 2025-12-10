using System.Threading.Tasks;

namespace Script
{
    public class Command_Shutdown : OnShutdown<CommandService>
    {
        public Command_Shutdown(Server server, CommandService service) : base(server, service)
        {
        }

        protected override Task StopBusinesses()
        {
            return Task.CompletedTask;
        }
    }
}