using System.Numerics;
using Data;
using Script;

namespace Script
{
    public class UserServiceScript : ServiceScript<UserService>
    {
        public UserServiceData usData { get { return this.service.sd; } }

        public log4net.ILog logger { get { return this.service.logger; } }

        public async Task<(ECode, Profile?)> QueryUserProfile(long userId)
        {
            var msgDb = new MsgQueryUserProfile();
            msgDb.userId = userId;

            var r = await this.service.connectToDatabaseService.SendAsync(MsgType._Database_QueryUserProfile, msgDb);
            if (r.err != ECode.Success)
            {
                this.service.logger.Error($"QueryUserProfile({userId}) r.err {r.err}");
                return (r.err, null);
            }

            var resDb = r.CastRes<ResQueryUserProfile>();

            Profile? profile = resDb.profile;
            if (profile != null)
            {
                if (profile.userId != userId)
                {
                    this.service.logger.Error($"QueryUserProfile({userId}) different profile.userId {profile.userId}");
                    return (ECode.Error, null);
                }

                profile.Ensure();
            }

            return (ECode.Success, profile);
        }

        public async Task<ECode> InsertUserProfile(Profile profile)
        {
            var msgDb = new MsgInsertUserProfile();
            msgDb.profile = profile;

            var r = await this.service.connectToDatabaseService.SendAsync(MsgType._Database_InsertUserProfile, msgDb);
            if (r.err != ECode.Success)
            {
                this.service.logger.Error($"InsertUserProfile({profile.userId}) r.err {r.err}");
                return r.err;
            }

            return ECode.Success;
        }

        public Profile NewProfile(long userId)
        {
            Profile profile = Profile.Ensure(null);
            profile.userId = userId;

            long nowS = TimeUtils.GetTimeS();
            profile.createTimeS = nowS;
            profile.lastLoginTimeS = nowS;
            return profile;
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