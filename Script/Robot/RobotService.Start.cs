using Data;

namespace Script
{
    public partial class RobotService
    {
        protected async override Task<ECode> Start2()
        {
            ECode e = await base.Start2();
            if (e != ECode.Success)
            {
                return e;
            }

            var socket = new TcpClientData();
            socket.ConnectorInit(this.sd.protocolClientCallbackProviderRobotClient, "localhost", 8001);
            var connection = new RobotClientConnection(socket, true);
            socket.Connect();
            return e;
        }
    }
}