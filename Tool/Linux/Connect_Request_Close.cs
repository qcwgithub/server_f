using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task<MyResponse> Connect_Request_Close(ServiceConfig serviceConfig, MsgType msgType, object msg)
        {
            var tai = serviceConfig.tai;

            var connection = new ToolConnection();
            bool success = await connection.Connect(serviceConfig.inIp, serviceConfig.inPort);
            if (!success)
            {
                ConsoleEx.WriteLine(ConsoleColor.Red, $"Connect to {tai} failed");
                return ECode.NotConnected;
            }
            ConsoleEx.WriteLine(ConsoleColor.Green, $"Connect to {tai} ok");

            var r = await connection.Request(msgType, msg);
            ConsoleEx.WriteLine(r.e == ECode.Success ? ConsoleColor.Green : ConsoleColor.Red, $"Request {msgType} {r.e}");
            connection.Close();

            Console.WriteLine();
            return r;
        }

        async Task Connect_Request_Close(List<ServiceConfig> serviceConfigs, MsgType msgType, object msg)
        {
            foreach (ServiceConfig serviceConfig in serviceConfigs)
            {
                await this.Connect_Request_Close(serviceConfig, msgType, msg);
            }
        }
    }
}