using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task<MyResponse> Connect_Request_Close(ServiceConfig serviceConfig, MsgType msgType, object msg)
        {
            var tai = serviceConfig.tai;

            var connectionCallback = new ToolConnectionCallback();
            var tcsConnect = new TaskCompletionSource<bool>();
            connectionCallback.onConnect = success => tcsConnect.SetResult(success);

            var connection = new ToolConnection(connectionCallback, serviceConfig.inIp, serviceConfig.inPort);
            connection.Connect();
            bool success = await tcsConnect.Task;
            if (!success)
            {
                ConsoleEx.WriteLine(ConsoleColor.Red, $"Connect to {tai} failed");
                return ECode.NotConnected;
            }
            ConsoleEx.WriteLine(ConsoleColor.Green, $"Connect to {tai} ok");
            connection.Send(MsgType._ConnectorInfo, new MsgConnectorInfo { isCommand = true }, null);

            var r = await connection.Request(msgType, msg);
            ConsoleEx.WriteLine(r.e == ECode.Success ? ConsoleColor.Green : ConsoleColor.Red, $"Request {msgType} {r.e}");
            connection.Close("Doesn't matter");

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