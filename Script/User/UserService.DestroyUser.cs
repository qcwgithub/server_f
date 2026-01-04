
using Data;

namespace Script
{
    public partial class UserService
    {
        public async Task<ECode> DestroyUser(User user, UserDestroyUserReason reason)
        {
            this.logger.InfoFormat("DestroyUser userId {0}, reason {1}, preCount {2}", user.userId, reason, this.sd.userCount);

            this.ss.ClearSaveTimer(user);

            // Save once
            ECode e = await this.SaveUser(user, "DestroyUser");
            if (e != ECode.Success)
            {
                return e;
            }

            this.sd.RemoveUser(user.userId);
            this.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}