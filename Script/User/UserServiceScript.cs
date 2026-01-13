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

            var r = await this.service.dbServiceProxy.Query_UserInfo_by_userId(msgDb);
            if (r.e != ECode.Success)
            {
                this.service.logger.Error($"QueryUserInfo({userId}) r.err {r.e}");
                return (r.e, null);
            }

            var resDb = r.CastRes<ResQuery_UserInfo_by_userId>();

            UserInfo? userInfo = resDb.result;
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

        public void SetSaveTimer(User user)
        {
            if (user.saveTimer.IsAlive())
            {
                return;
            }

            var SEC = this.usData.saveIntervalS;
#if DEBUG
            SEC = 3;
#endif
            user.saveTimer = server.timerScript.SetLoopTimer(this.service.serviceId, SEC, TimerType.SaveUser, new TimerSaveUser
            {
                userId = user.userId,
                reason = "SetSaveTimer"
            });
        }

        public void ClearSaveTimer(User user)
        {
            if (user.saveTimer == null)
            {
                return;
            }

            if (!user.saveTimer.IsAlive())
            {
                return;
            }

            server.timerScript.ClearTimer(user.saveTimer);
            user.saveTimer = null;
        }

        public void SetDestroyTimer(User user, UserDestroyUserReason reason)
        {
            if (user.destroyTimer.IsAlive())
            {
                return;
            }

            var SEC = this.service.sd.destroyTimeoutS;
            this.service.logger.Info($"SetDestroyTimer userId {user.userId} reason {reason}");

            user.destroyTimer = this.server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, TimerType.DestroyUser,
                new TimerDestroyUser
                {
                    userId = user.userId,
                    reason = reason
                });
        }

        public void ClearDestroyTimer(User user, UserClearDestroyTimerReason reason)
        {
            if (user.destroyTimer == null)
            {
                return;
            }

            if (!user.destroyTimer.IsAlive())
            {
                return;
            }

            this.service.logger.Info($"ClearDestroyTimer userId {user.userId} reason {reason}");
            server.timerScript.ClearTimer(user.destroyTimer);
            user.destroyTimer = null;
        }
    }
}