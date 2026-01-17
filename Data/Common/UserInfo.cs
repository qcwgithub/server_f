using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserInfo
    {
        #region auto

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
        }

        #endregion auto
    }
}