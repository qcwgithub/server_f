using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task ShutdownServices(bool all)
        {
            string joinedServices;
            if (!all)
            {
                joinedServices = await this.SelectServices(null, true);
            }
            else
            {
                List<string[]> runningServices = this.GetRunningServices();
                joinedServices = string.Join(',', runningServices.Select(array => string.Join(',', array)).ToArray());
            }
            await this.ConfirmShutdownServices(joinedServices);
        }

        async Task ConfirmShutdownServices(string joinedServices)
        {
            (int index, string answer) = AskHelp.AskSelect("Are you sure to shutdown " + joinedServices + "?", "no", "yes", "yes and force")
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

            List<ServiceTypeAndId> tais = joinedServices.Split(',').Select(s => ServiceTypeAndId.FromString(s)).ToList();
            tais.Sort((a, b) =>
            {
                int a_order = ServerData.shutdownServiceOrder.IndexOf(a.serviceType);
                int b_order = ServerData.shutdownServiceOrder.IndexOf(b.serviceType);
                return a_order - b_order;
            });

            for (int i = 0; i < tais.Count; i++)
            {
                ServiceTypeAndId tai = tais[i];
                ServiceConfig? serviceConfig = this.allServiceConfigs.Find(x => x.serviceType == tai.serviceType && x.serviceId == tai.serviceId);
                if (serviceConfig == null)
                {
                    ConsoleEx.WriteLine(ConsoleColor.Red, $"serviceConfig == null, tai {tai}");
                    continue;
                }

                ConsoleEx.WriteLine(ConsoleColor.White, $"Connecting to {tai}...");
                var connection = new ToolConnection();
                bool success = await connection.Connect(serviceConfig.inIp, serviceConfig.inPort);
                if (!success)
                {
                    ConsoleEx.WriteLine(ConsoleColor.Red, $"Connect to {tai} failed");
                    continue;
                }
                ConsoleEx.WriteLine(ConsoleColor.Green, $"Connect to {tai} ok");

                MsgType msgType = MsgType._Service_Shutdown;
                var r = await connection.Request(msgType, new MsgShutdown { force = option == 3 });
                ConsoleEx.WriteLine(r.e == ECode.Success ? ConsoleColor.Green : ConsoleColor.Red, $"Request {msgType} result {r.e}");
                connection.Close();

                Console.WriteLine();
            }
        }
    }
}