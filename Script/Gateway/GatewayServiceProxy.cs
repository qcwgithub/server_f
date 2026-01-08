using Data;

namespace Script
{
    public class GatewayServiceProxy : ServiceProxy
    {
        public GatewayServiceProxy(Service self) : base(self, ServiceType.Gateway)
        {
        }

        public ECode BroadcastToClient(int serviceId, List<long> userIds, MsgType msgType, object msg)
        {
            ServiceConnection? connection = this.self.data.GetOtherServiceConnection(serviceId);
            if (connection == null || !connection.IsConnected())
            {
                return ECode.Server_NotConnected;
            }

            if (connection == null)
            {
                return ECode.Server_NotConnected;
            }

            Forwarding.S_to_G(connection, userIds, MsgType.A_RoomChat, msg, null, null);
            return ECode.Success;
        }

        #region auto_proxy

        public async Task<MyResponse> ServerAction(int serviceId, MsgGatewayServiceAction msg)
        {
            return await this.Request(serviceId, MsgType._Gateway_ServerAction, msg);
        }
        public async Task<MyResponse> ServerKick(int serviceId, MsgGatewayServerKick msg)
        {
            return await this.Request(serviceId, MsgType._Gateway_ServerKick, msg);
        }

        #endregion auto_proxy
    }
}