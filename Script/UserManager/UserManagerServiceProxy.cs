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
        public async Task<MyResponse> GetUserLocation(MsgUserManagerGetUserLocation msg)
        {
            return await this.Request(ServiceType.UserManager, MsgType._UserManager_GetUserLocation, msg);
        }
        public async Task<MyResponse> ForwardToUserService(MsgForwardToUserService msg)
        {
            return await this.Request(ServiceType.UserManager, MsgType._UserManager_ForwardToUserService, msg);
        }

        #endregion auto_proxy

        public async Task<MyResponse> ForwardToUserService(long toUserId, MsgType innerMsgType, object innerMsg, bool simulateLoginIfOffline)
        {
            var msg = new MsgForwardToUserService();
            msg.userId = toUserId;
            msg.innerMsgType = innerMsgType;
            msg.innerMsgBytes = MessageTypeConfigData.SerializeMsg(innerMsgType, innerMsg);
            msg.simulateLoginIfOffline = simulateLoginIfOffline;

            var r = await this.ForwardToUserService(msg);

            var res = r.CastRes<ResForwardToUserService>();
            var innerRes = MessageTypeConfigData.DeserializeRes(innerMsgType, res.innerResBytes);
            return new MyResponse(r.e, innerRes);
        }
    }
}