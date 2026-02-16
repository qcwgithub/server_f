using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserInfo : IIsDifferent<UserInfo>
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string userName;
        [Key(2)]
        public long createTimeS;
        [Key(3)]
        public long lastLoginTimeS;
        [Key(4)]
        public long lastSetNameTimeS;
        [Key(5)]
        public int avatarIndex;
        [Key(6)]
        public long lastSetAvatarIndexTimeS;
        [Key(7)]
        public List<FriendInfo> friends;
        [Key(8)]
        public List<OutgoingFriendRequest> outgoingFriendRequests;
        [Key(9)]
        public List<IncomingFriendRequest> incomingFriendRequests;
        [Key(10)]
        public List<BlockedUser> blockedUsers;

        public static UserInfo Ensure(UserInfo? p)
        {
            if (p == null)
            {
                p = new UserInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.userName == null)
            {
                this.userName = string.Empty;
            }
            if (this.friends == null)
            {
                this.friends = new List<FriendInfo>();
            }
            for (int i = 0; i < this.friends.Count; i++)
            {
                this.friends[i] = FriendInfo.Ensure(this.friends[i]);
            }
            if (this.outgoingFriendRequests == null)
            {
                this.outgoingFriendRequests = new List<OutgoingFriendRequest>();
            }
            for (int i = 0; i < this.outgoingFriendRequests.Count; i++)
            {
                this.outgoingFriendRequests[i] = OutgoingFriendRequest.Ensure(this.outgoingFriendRequests[i]);
            }
            if (this.incomingFriendRequests == null)
            {
                this.incomingFriendRequests = new List<IncomingFriendRequest>();
            }
            for (int i = 0; i < this.incomingFriendRequests.Count; i++)
            {
                this.incomingFriendRequests[i] = IncomingFriendRequest.Ensure(this.incomingFriendRequests[i]);
            }
            if (this.blockedUsers == null)
            {
                this.blockedUsers = new List<BlockedUser>();
            }
            for (int i = 0; i < this.blockedUsers.Count; i++)
            {
                this.blockedUsers[i] = BlockedUser.Ensure(this.blockedUsers[i]);
            }
        }

        public bool IsDifferent(UserInfo other)
        {
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.userName != other.userName)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.lastLoginTimeS != other.lastLoginTimeS)
            {
                return true;
            }
            if (this.lastSetNameTimeS != other.lastSetNameTimeS)
            {
                return true;
            }
            if (this.avatarIndex != other.avatarIndex)
            {
                return true;
            }
            if (this.lastSetAvatarIndexTimeS != other.lastSetAvatarIndexTimeS)
            {
                return true;
            }
            if (this.friends.IsDifferent_ListClass(other.friends))
            {
                return true;
            }
            if (this.outgoingFriendRequests.IsDifferent_ListClass(other.outgoingFriendRequests))
            {
                return true;
            }
            if (this.incomingFriendRequests.IsDifferent_ListClass(other.incomingFriendRequests))
            {
                return true;
            }
            if (this.blockedUsers.IsDifferent_ListClass(other.blockedUsers))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(UserInfo other)
        {
            this.userId = other.userId;
            this.userName = other.userName;
            this.createTimeS = other.createTimeS;
            this.lastLoginTimeS = other.lastLoginTimeS;
            this.lastSetNameTimeS = other.lastSetNameTimeS;
            this.avatarIndex = other.avatarIndex;
            this.lastSetAvatarIndexTimeS = other.lastSetAvatarIndexTimeS;
            this.friends.DeepCopyFrom_ListClass(other.friends);
            this.outgoingFriendRequests.DeepCopyFrom_ListClass(other.outgoingFriendRequests);
            this.incomingFriendRequests.DeepCopyFrom_ListClass(other.incomingFriendRequests);
            this.blockedUsers.DeepCopyFrom_ListClass(other.blockedUsers);
        }
    }
}
