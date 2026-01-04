using Data;

namespace Script
{
    public class UserManagerServiceProxy : ServiceProxy
    {
        public UserManagerServiceProxy(Service self) : base(self, ServiceType.UserManager)
        {
        }

        #region auto_proxy

        public async Task<MyResponse> UserLogin(MsgUserManagerUserLogin msg)
        {
            return await this.Request(ServiceType.UserManager, MsgType._UserManager_UserLogin, msg);
        }

        #endregion auto_proxy
    }
}