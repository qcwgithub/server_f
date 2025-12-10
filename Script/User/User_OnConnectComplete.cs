using Data;

namespace Script
{
    // 连接其他服务器成功
    public class User_OnConnectComplete : OnConnectComplete<UserService>
    {
        public User_OnConnectComplete(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgConnectorInfo msg, ResConnectorInfo res)
        {
            var e = await base.Handle(socket, msg, res);
            if (e != ECode.Success)
            {
                return e;
            }

            if (socket.serviceTypeAndId == null)
            {
                return e;
            }

            // var serviceTypeAndId = (ServiceTypeAndId)socket.serviceTypeAndId;

            // if (serviceTypeAndId.serviceType == ServiceType.Stateless)
            // {
            //     await this.service.SendPSInfoToAAA(false, socket);
            // }

            return e;
        }
    }
}