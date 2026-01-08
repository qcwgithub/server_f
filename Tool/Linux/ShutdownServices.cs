using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task ShutdownServices(bool all)
        {
            List<ServiceConfig> serviceConfigs;
            if (!all)
            {
                serviceConfigs = this.SelectServices(null, true);
            }
            else
            {
                List<string[]> runningServices = this.GetRunningServices();
                serviceConfigs = this.FindServiceConfigs(runningServices);
            }
            await this.ConfirmShutdownServices(serviceConfigs);
        }

        async Task ConfirmShutdownServices(List<ServiceConfig> serviceConfigs)
        {
            (int index, string answer) = AskHelp
                .AskSelect("Are you sure to shutdown " + string.Join(',', serviceConfigs.Select(x => x.tai.ToString())) + "?", "no", "yes", "yes and force")
                .OnAnswer2();

            int option;
            if (answer.StartsWith("yes"))
            {
                option = (answer == "yes" ? 2 : 3);
            }
            else
            {
                return;
            }

            serviceConfigs.Sort((a, b) =>
            {
                int a_order = ServerData.shutdownServiceOrder.IndexOf(a.serviceType);
                int b_order = ServerData.shutdownServiceOrder.IndexOf(b.serviceType);
                return a_order - b_order;
            });

            var msg = new MsgShutdown { force = option == 3 };
            await this.Connect_Request_Close(serviceConfigs, MsgType._Service_Shutdown, msg);
        }
    }
}