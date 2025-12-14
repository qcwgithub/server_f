using System.Numerics;
using Data;
using Script;

namespace Script
{
    public class UserServiceScript : ServiceScript<UserService>
    {
        public UserServiceScript(Server server, UserService service) : base(server, service)
        {
        }


        public UserServiceData usData { get { return this.service.sd; } }

        public log4net.ILog logger { get { return this.service.logger; } }

        public async Task<(ECode, UserInfo?)> QueryUserInfo(long userId)
        {
            var msgDb = new MsgQuery_UserInfo_by_userId();
            msgDb.userId = userId;

            var r = await this.service.connectToDbService.Request<MsgQuery_UserInfo_by_userId, ResQuery_UserInfo_by_userId>(MsgType._Query_UserInfo_by_userId, msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryUserInfo({userId}) r.err {r.e}");
                return (r.e, null);
            }

            UserInfo? userInfo = r.res.result;
            if (userInfo != null)
            {
                if (userInfo.userId != userId)
                {
                    this.service.logger.Error($"QueryUserUserInfo({userId}) different userInfo.userId {userInfo.userId}");
                    return (ECode.Error, null);
                }

                userInfo.Ensure();
            }

            return (ECode.Success, userInfo);
        }

        public async Task<ECode> InsertUserInfo(UserInfo userInfo)
        {
            var msgDb = new MsgInsertUserInfo();
            msgDb.userInfo = userInfo;

            var r = await this.service.connectToDbService.Request<MsgInsertUserInfo, ResInsertUserInfo>(MsgType._Db_InsertUserInfo, msgDb);
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
            return userInfo;
        }

        public void SetDestroyTimer(User user, string place)
        {
            MyDebug.Assert(!user.destroyTimer.IsAlive());

            var SEC = this.usData.destroyTimeoutS;
            this.logger.InfoFormat("SetDestroyTimer userId({1}), seconds({2}), reason({3})", place, user.userId, SEC, place);

            user.destroyTimer = server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, MsgType._User_DestroyUser,
                MsgDestroyUser.Create(user.userId, "SetDestroyTimer", null));
        }

        public void ClearDestroyTimer(User user, bool log)
        {
            MyDebug.Assert(user.destroyTimer.IsAlive());

            if (log)
            {
                this.logger.InfoFormat("ClearDestroyTimer userId({0})", user.userId);
            }
            server.timerScript.ClearTimer(user.destroyTimer);
            user.destroyTimer = null;
        }

        public void SetSaveTimer(User user)
        {
            MyDebug.Assert(!user.saveTimer.IsAlive());

            var SEC = this.usData.saveIntervalS;
#if DEBUG
            SEC = 3;
#endif

            var msg = new MsgSaveUser { userId = user.userId, place = "timer" };
            user.saveTimer = server.timerScript.SetLoopTimer(this.service.serviceId, SEC, MsgType._User_SaveUser, msg);
        }

        public void ClearSaveTimer(User user)
        {
            MyDebug.Assert(user.saveTimer.IsAlive());

            server.timerScript.ClearTimer(user.saveTimer);
            user.saveTimer = null;
        }
    }
}