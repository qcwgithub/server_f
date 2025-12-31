using Data;

namespace Script
{
    public class UserManagerServiceProxy : ServiceProxy
    {
        public UserManagerServiceProxy(Service self) : base(self, ServiceType.UserManager)
        {
        }

        #region auto_proxy

        public async Task<MyResponse<ResUserManagerUserLogin>> UserLogin(MsgUserManagerUserLogin msg)
        {
            return await this.Request<MsgUserManagerUserLogin, ResUserManagerUserLogin>(ServiceType.UserManager, MsgType._UserManager_UserLogin, msg);
        }

        #endregion auto_proxy
    }
}