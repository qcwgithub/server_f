using Data;

namespace Script
{
    public class ConnectWithUserService : ConnectToOtherService
    {
        public ConnectWithUserService(Service self, ServiceType to) : base(self, to)
        {
        }

        // 发送给 UserService 必须指定 serviceId
        public async Task<MyResponse<Res>> Request<Msg, Res>(int serviceId, MsgType msgType, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(msgType, msg);
        }
    }
}