using Data;

namespace Script
{
    public class ConnectToUserManagerService : ConnectToStatelessService
    {
        public ConnectToUserManagerService(Service self) : base(self, ServiceType.UserManager)
        {

        }

        public async Task<MyResponse<ResUserManagerUserLogin>> UserLogin(MsgUserManagerUserLogin msg)
        {
            return await this.Request<MsgUserManagerUserLogin, ResUserManagerUserLogin>(MsgType._UserManager_UserLogin, msg);
        }
    }
}