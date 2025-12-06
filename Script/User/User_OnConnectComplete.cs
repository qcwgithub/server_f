using Data;

namespace Script
{
    // 连接其他服务器成功
    public class User_OnConnectComplete : OnConnectComplete<UserService>
    {
        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgConnectorInfo msg)
        {
            MyResponse r = await base.Handle(socket, msg);
            if (r.err != ECode.Success)
            {
                return r;
            }

            if (socket.serviceTypeAndId == null)
            {
                return r;
            }

            // var serviceTypeAndId = (ServiceTypeAndId)socket.serviceTypeAndId;

            // if (serviceTypeAndId.serviceType == ServiceType.Stateless)
            // {
            //     await this.service.SendPSInfoToAAA(false, socket);
            // }

            return r;
        }
    }
}