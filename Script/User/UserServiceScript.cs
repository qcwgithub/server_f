using System.Numerics;
using Data;
using Script;

namespace Script
{
    public class UserServiceScript : ServiceScript<UserService>
    {
        public UserServiceData usData { get { return this.service.usData; } }

        public log4net.ILog logger { get { return this.service.logger; } }

        public void SetDestroyTimer(User user, string place)
        {
            MyDebug.Assert(!user.destroyTimer.IsAlive());

            var SEC = this.usData.playerDestroyTimeoutS;
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

            var SEC = this.usData.playerSaveIntervalS;
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