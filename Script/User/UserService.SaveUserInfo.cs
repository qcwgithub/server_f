using Data;

namespace Script
{
    public partial class UserService
    {
        public async Task<ECode> SaveUserInfo(User user, string reason)
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
            if (last.lastSetNameTimeS != curr.lastSetNameTimeS)
            {
                infoNullable.lastSetNameTimeS = curr.lastSetNameTimeS;
                last.lastSetNameTimeS = curr.lastSetNameTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("lastSetNameTimeS");
            }
            if (last.avatarIndex != curr.avatarIndex)
            {
                infoNullable.avatarIndex = curr.avatarIndex;
                last.avatarIndex = curr.avatarIndex;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("avatarIndex");
            }
            if (last.lastSetAvatarIndexTimeS != curr.lastSetAvatarIndexTimeS)
            {
                infoNullable.lastSetAvatarIndexTimeS = curr.lastSetAvatarIndexTimeS;
                last.lastSetAvatarIndexTimeS = curr.lastSetAvatarIndexTimeS;
                if (buffer == null) buffer = new List<string>();
                buffer.Add("lastSetAvatarIndexTimeS");
            }
            if (last.friends.IsDifferent_ListClass(curr.friends))
            {
                infoNullable.friends = curr.friends;
                last.friends.DeepCopyFrom_ListClass(curr.friends);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("friends");
            }
            if (last.outgoingFriendRequests.IsDifferent_ListClass(curr.outgoingFriendRequests))
            {
                infoNullable.outgoingFriendRequests = curr.outgoingFriendRequests;
                last.outgoingFriendRequests.DeepCopyFrom_ListClass(curr.outgoingFriendRequests);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("outgoingFriendRequests");
            }
            if (last.incomingFriendRequests.IsDifferent_ListClass(curr.incomingFriendRequests))
            {
                infoNullable.incomingFriendRequests = curr.incomingFriendRequests;
                last.incomingFriendRequests.DeepCopyFrom_ListClass(curr.incomingFriendRequests);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("incomingFriendRequests");
            }
            if (last.blockedUsers.IsDifferent_ListClass(curr.blockedUsers))
            {
                infoNullable.blockedUsers = curr.blockedUsers;
                last.blockedUsers.DeepCopyFrom_ListClass(curr.blockedUsers);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("blockedUsers");
            }
            if (last.removedFriends.IsDifferent_ListClass(curr.removedFriends))
            {
                infoNullable.removedFriends = curr.removedFriends;
                last.removedFriends.DeepCopyFrom_ListClass(curr.removedFriends);
                if (buffer == null) buffer = new List<string>();
                buffer.Add("removedFriends");
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

                var userBriefInfo = UserServiceScript.CreateUserBriefInfo(user.userInfo);
                if (user.lastBriefInfo == null || user.lastBriefInfo.IsDifferent(userBriefInfo))
                {
                    user.lastBriefInfo = userBriefInfo;
                    this.server.userBriefInfoProxy.Save(userBriefInfo).Forget();
                }
            }

            return ECode.Success;
        }
    }
}