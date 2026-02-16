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

        public static string DefaultUserName(long createTimeS)
        {
            return "User_" + createTimeS;
        }

        public UserInfo NewUserInfo(long userId)
        {
            UserInfo userInfo = UserInfo.Ensure(null);
            userInfo.userId = userId;

            long nowS = TimeUtils.GetTimeS();
            userInfo.createTimeS = nowS;
            userInfo.lastLoginTimeS = nowS;
            userInfo.userName = DefaultUserName(nowS);
            userInfo.avatarIndex = this.server.data.random.Next(
                this.server.data.serverConfig.userAvatarConfig.minIndex,
                this.server.data.serverConfig.userAvatarConfig.maxIndex + 1
            );
            return userInfo;
        }

        public struct stCheckUserExistAndAddLocationResult
        {
            public ECode e;
            public string channel;
            public string channelUserId;
            public stObjectLocation location;
        }

        public async Task<stCheckUserExistAndAddLocationResult> CheckUserExistAndAddLocation(MessageContext context, long userId)
        {
            var ret = new stCheckUserExistAndAddLocationResult();

            var msgDb = new MsgQuery_AccountInfo_byElementOf_userIds();
            msgDb.ele_userIds = userId;

            var r = await this.service.dbServiceProxy.Query_AccountInfo_byElementOf_userIds(msgDb);
            if (r.e != ECode.Success)
            {
                ret.e = r.e;
                return ret;
            }

            var resDb = r.CastRes<ResQuery_AccountInfo_byElementOf_userIds>();
            if (resDb.result == null)
            {
                ret.e = ECode.AccountNotExist;
                return ret;
            }

            ret.channel = resDb.result.channel;
            ret.channelUserId = resDb.result.channelUserId;

            context.lockValue = await this.server.lockRedis.LockAccount(ret.channel, ret.channelUserId, this.service.logger);
            if (context.lockValue == null)
            {
                ret.e = ECode.RedisLockFail;
                return ret;
            }

            ret.location = await this.service.userLocationAssignmentScript.AssignLocation(userId);
            if (!ret.location.IsValid())
            {
                ret.e = ECode.NoAvailableUserService;
                return ret;
            }

            this.service.userLocator.CacheLocation(userId, ret.location);
            return ret;
        }
    }
}