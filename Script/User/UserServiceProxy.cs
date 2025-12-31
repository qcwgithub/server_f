using Data;

namespace Script
{
    public class UserServiceProxy : ServiceProxy
    {
        public UserServiceProxy(Service self) : base(self, ServiceType.User)
        {
        }

        #region auto_proxy

        public async Task<MyResponse<ResUserServiceAction>> ServerAction(int serviceId, MsgUserServiceAction msg)
        {
            return await this.Request<MsgUserServiceAction, ResUserServiceAction>(serviceId, MsgType._User_ServerAction, msg);
        }

        public async Task<MyResponse<ResUserLoginSuccess>> UserLoginSuccess(int serviceId, MsgUserLoginSuccess msg)
        {
            return await this.Request<MsgUserLoginSuccess, ResUserLoginSuccess>(serviceId, MsgType._User_UserLoginSuccess, msg);
        }

        public async Task<MyResponse<ResUserServerKick>> ServerKick(int serviceId, MsgUserServerKick msg)
        {
            return await this.Request<MsgUserServerKick, ResUserServerKick>(serviceId, MsgType._User_ServerKick, msg);
        }

        public async Task<MyResponse<ResUserDisconnectFromGateway>> UserDisconnectFromGateway(int serviceId, MsgUserDisconnectFromGateway msg)
        {
            return await this.Request<MsgUserDisconnectFromGateway, ResUserDisconnectFromGateway>(serviceId, MsgType._User_UserDisconnectFromGateway, msg);
        }

        public async Task<MyResponse<ResSaveUser>> SaveUserImmediately(int serviceId, MsgSaveUser msg)
        {
            return await this.Request<MsgSaveUser, ResSaveUser>(serviceId, MsgType._User_SaveUserImmediately, msg);
        }

        public async Task<MyResponse<ResGetUserCount>> GetUserCount(int serviceId, MsgGetUserCount msg)
        {
            return await this.Request<MsgGetUserCount, ResGetUserCount>(serviceId, MsgType._User_GetUserCount, msg);
        }

        public async Task<MyResponse<ResSaveUserInfoToFile>> SaveUserInfoToFile(int serviceId, MsgSaveUserInfoToFile msg)
        {
            return await this.Request<MsgSaveUserInfoToFile, ResSaveUserInfoToFile>(serviceId, MsgType._User_SaveUserInfoToFile, msg);
        }

        public async Task<MyResponse<ResSetGmFlag>> SetGmFlag(int serviceId, MsgSetGmFlag msg)
        {
            return await this.Request<MsgSetGmFlag, ResSetGmFlag>(serviceId, MsgType._User_SetGmFlag, msg);
        }


        #endregion auto_proxy
    }
}