using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Global_Shutdown : OnShutdown<GlobalService>
    {
        public Global_Shutdown(Server server, GlobalService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {
            var sd = this.service.globalServiceData;
            OnShutdown<Service>.s_ClearTimer(this.server, ref sd.timer_tick_Loop);
        }
    }
}