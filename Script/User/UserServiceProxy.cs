using Data;

namespace Script
{
    public class UserServiceProxy : ServiceProxy
    {
        public UserServiceProxy(Service self) : base(self, ServiceType.User)
        {
        }

        #region auto_proxy

        public async Task<MyResponse> ServerAction(int serviceId, MsgUserServiceAction msg)
        {
            return await this.Request(serviceId, MsgType._User_ServerAction, msg);
        }
        public async Task<MyResponse> UserLoginSuccess(int serviceId, MsgUserLoginSuccess msg)
        {
            return await this.Request(serviceId, MsgType._User_UserLoginSuccess, msg);
        }
        public async Task<MyResponse> ServerKick(int serviceId, MsgUserServerKick msg)
        {
            return await this.Request(serviceId, MsgType._User_ServerKick, msg);
        }
        public async Task<MyResponse> UserDisconnectFromGateway(int serviceId, MsgUserDisconnectFromGateway msg)
        {
            return await this.Request(serviceId, MsgType._User_UserDisconnectFromGateway, msg);
        }
        public async Task<MyResponse> SaveUserImmediately(int serviceId, MsgSaveUserImmediately msg)
        {
            return await this.Request(serviceId, MsgType._User_SaveUserImmediately, msg);
        }
        public async Task<MyResponse> GetUserCount(int serviceId, MsgGetUserCount msg)
        {
            return await this.Request(serviceId, MsgType._User_GetUserCount, msg);
        }
        public async Task<MyResponse> SaveUserInfoToFile(int serviceId, MsgSaveUserInfoToFile msg)
        {
            return await this.Request(serviceId, MsgType._User_SaveUserInfoToFile, msg);
        }
        public async Task<MyResponse> SetGmFlag(int serviceId, MsgSetGmFlag msg)
        {
            return await this.Request(serviceId, MsgType._User_SetGmFlag, msg);
        }

        #endregion auto_proxy
    }
}