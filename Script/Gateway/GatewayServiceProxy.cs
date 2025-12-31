using Data;

namespace Script
{
    public class GatewayServiceProxy : ServiceProxy
    {
        public GatewayServiceProxy(Service self) : base(self, ServiceType.Gateway)
        {
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