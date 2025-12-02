using System.Threading.Tasks;

namespace Script
{
    public class Command_Shutdown : OnShutdown<CommandService>
    {
        protected override Task StopBusinesses()
        {
            return Task.CompletedTask;
        }
    }
}