using Data;

namespace Script
{
    public partial class UserService
    {
        public async Task<ECode> SaveUser(User user, string reason)
        {
            await this.server.userLocationRedisW.WriteLocation(user.userId, this.serviceId, this.sd.saveIntervalS + 60);

            var msgDb = new MsgSave_UserInfo
            {
                userId = user.userId,
                userInfoNullable = new UserInfoNullable()
            };
            var infoNullable = msgDb.userInfoNullable;

            List<string>? buffer = null;
            if (user.lastUserInfo == null)
            {
                this.logger.Error($"SaveUser user.lastUserInfo == null");
                return ECode.Error;
            }

            UserInfo last = user.lastUserInfo;
            UserInfo curr = user.userInfo;

            #region auto

            if (last.userId != curr.userId)
            {
                infoNullable.userId = curr.userId;
                last.userId = curr.userId;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userId");
            }
            if (last.userName != curr.userName)
            {
                infoNullable.userName = curr.userName;
                last.userName = curr.userName;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("userName");
            }
            if (last.createTimeS != curr.createTimeS)
            {
                infoNullable.createTimeS = curr.createTimeS;
                last.createTimeS = curr.createTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("createTimeS");
            }
            if (last.lastLoginTimeS != curr.lastLoginTimeS)
            {
                infoNullable.lastLoginTimeS = curr.lastLoginTimeS;
                last.lastLoginTimeS = curr.lastLoginTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("lastLoginTimeS");
            }

            #endregion auto

            // player.lastUserInfo = curr; // 先假设一定成功吧
            if (last.IsDifferent(curr))
            {
                this.logger.Error("last.IsDifferent(curr)!!!");
            }

            string fieldsStr = "";
            if (buffer != null)
            {
                fieldsStr = string.Join(", ", buffer.ToArray());

                // buffer 不为 null 才打印，不然太多了
                this.logger.InfoFormat("SaveUser userId {0}, reason {1}, fields [{2}]", user.userId, reason, fieldsStr);
            }

            if (buffer != null)
            {
#if DEBUG
                msgDb.userInfo_debug = UserInfo.Ensure(null);
                msgDb.userInfo_debug.DeepCopyFrom(curr);
#endif
                var r = await this.dbServiceProxy.Save_UserInfo(msgDb);
                if (r.e != ECode.Success)
                {
                    this.logger.ErrorFormat("Save_UserInfo e {0}, userId {1}", r.e, user.userId);
                    return r.e;
                }
            }

            return ECode.Success;
        }
    }
}