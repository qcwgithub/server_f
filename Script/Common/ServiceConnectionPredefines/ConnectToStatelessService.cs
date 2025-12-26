using Data;

namespace Script
{
    public abstract class ConnectToStatelessService : ConnectToOtherService
    {
        public ConnectToStatelessService(Service self, ServiceType to) : base(self, to)
        {
        }

        async Task<MyResponse<Res>> Request<Msg, Res>(ServiceType serviceType, MsgType type, Msg msg)
            where Res : class
        {
            IConnection? connection = this.self.protocolClientScriptForS.RandomOtherServiceConnection(serviceType);
            if (connection == null)
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            return await connection.Request<Msg, Res>(type, msg);
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(MsgType msgType, Msg msg)
            where Res : class
        {
            return await this.Request<Msg, Res>(this.to, msgType, msg);
        }
    }
}