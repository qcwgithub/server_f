using Data;

namespace Script
{
    public partial class UserService
    {
        // Should lock before call
        public async Task<(ECode, User?)> LoadUser(long userId, UserInfo? userInfo)
        {
            ECode e;
            if (userInfo == null)
            {
                (e, userInfo) = await this.ss.QueryUserInfo(userId);
                if (e != ECode.Success)
                {
                    return (e, null);
                }

                if (userInfo == null)
                {
                    return (ECode.UserInfoNotExist, null);
                }
            }

            User user = new User(userInfo);

            await this.server.userLocationRedisW.WriteLocation(userId, this.serviceId, this.sd.saveIntervalS + 60);

            this.AddUserToDict(user);

            return (ECode.Success, user);
        }

        void AddUserToDict(User user)
        {
            // runtime 初始化
            this.sd.AddUser(user);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(user.lastUserInfo == null);

            user.lastUserInfo = UserInfo.Ensure(null);
            user.lastUserInfo.DeepCopyFrom(user.userInfo);

            user.lastBriefInfo = user.CreateUserBriefInfo();

            // qiucw
            // 这句会修改 userInfo，必须放在 lastUserInfo.DeepCopyFrom 后面
            // this.gameScripts.CallInit(user);
            this.CheckUpdateRuntimeInfo().Forget();
        }
    }
}