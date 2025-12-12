using Data;

namespace Script
{
    // 连接其他服务器成功
    public class User_OnConnectComplete : OnConnectComplete<UserService>
    {
        public User_OnConnectComplete(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgConnectorInfo msg, ResConnectorInfo res)
        {
            var e = await base.Handle(connection, msg, res);
            if (e != ECode.Success)
            {
                return e;
            }

            var serviceConnection = (ServiceConnection)connection;
            if (serviceConnection.serviceTypeAndId == null)
            {
                return e;
            }

            // var serviceTypeAndId = (ServiceTypeAndId)connection.serviceTypeAndId;

            // if (serviceTypeAndId.serviceType == ServiceType.Stateless)
            // {
            //     await this.service.SendPSInfoToAAA(false, connection);
            // }

            return e;
        }
    }
}