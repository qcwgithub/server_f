using Data;

namespace Script
{
    public abstract class ConnectToStatefulService : ConnectToOtherService
    {
        public ConnectToStatefulService(Service self, ServiceType to) : base(self, to)
        {
        }

        public async Task<MyResponse<Res>> Request<Msg, Res>(int serviceId, MsgType type, Msg msg)
            where Res : class
        {
            ServiceConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return new MyResponse<Res>(ECode.Server_NotConnected, null);
            }

            MyDebug.Assert(connection.serviceType == this.to);

            return await connection.Request<Msg, Res>(type, msg);
        }

        public int GetFirstConnected()
        {
            List<ServiceConnection> connections = this.self.data.otherServiceConnections2[(int)this.to];
            if (connections == null || connections.Count == 0)
            {
                return 0;
            }

            foreach (ServiceConnection connection in connections)
            {
                if (connection.IsConnected())
                {
                    return connection.serviceId;
                }
            }

            return 0;
        }
    }
}