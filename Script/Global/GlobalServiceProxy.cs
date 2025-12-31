using Data;

namespace Script
{
    public class GlobalServiceProxy : ServiceProxy
    {
        public GlobalServiceProxy(Service self) : base(self, ServiceType.Global)
        {
        }

        #region auto_proxy

        public async Task<MyResponse> GetServiceConfigs(MsgGetServiceConfigs msg)
        {
            return await this.Request(ServiceType.Global, MsgType._Global_GetServiceConfigs, msg);
        }

        #endregion auto_proxy
    }
}