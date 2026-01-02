
using Data;

namespace Script
{
    public partial class UserService
    {
        public async Task<ECode> DestroyUser(long userId, UserDestroyUserReason reason)
        {
            this.logger.InfoFormat("DestroyUser userId {0}, reason {1}, preCount {2}", userId, reason, this.sd.userCount);

            User? user = this.sd.GetUser(userId);
            if (user == null)
            {
                logger.InfoFormat("DestroyUser user not exist, userId: {0}", userId);
                return ECode.UserNotExist;
            }

            this.ss.ClearSaveTimer(user);

            user.destroying = true;

            // Save once
            ECode e = await this.SaveUser(userId, "DestroyUser");
            if (e != ECode.Success)
            {
                return e;
            }

            this.sd.RemoveUser(userId);
            this.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}