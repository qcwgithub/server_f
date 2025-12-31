using Data;

namespace Script
{
    public class CommandConnectToOtherService
    {
        Service self;
        public CommandConnectToOtherService(Service self)
        {
            this.self = self;
        }

        public async Task<MyResponse> Request(int serviceId, MsgType msgType, object msg)
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse(ECode.Server_NotConnected);
            }

            return await connection.Request(msgType, msg);
        }
    }
}