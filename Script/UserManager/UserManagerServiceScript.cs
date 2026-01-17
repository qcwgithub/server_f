using Data;

namespace Script
{
    public class UserManagerServiceScript : ServiceScript<UserManagerService>
    {
        public UserManagerServiceScript(Server server, UserManagerService service) : base(server, service)
        {
        }

        public async Task<ECode> InsertUserInfo(UserInfo userInfo)
        {
            var msgDb = new MsgInsert_UserInfo();
            msgDb.userInfo = userInfo;

            var r = await this.service.dbServiceProxy.InsertUserInfo(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"InsertUserInfo({userInfo.userId}) r.e {r.e}");
                return r.e;
            }

            return ECode.Success;
        }

        public UserInfo NewUserInfo(long userId)
        {
            UserInfo userInfo = UserInfo.Ensure(null);
            userInfo.userId = userId;

            long nowS = TimeUtils.GetTimeS();
            userInfo.createTimeS = nowS;
            userInfo.lastLoginTimeS = nowS;
            userInfo.userName = nowS.ToString();
            return userInfo;
        }
    }
}