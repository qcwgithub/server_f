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
            return false;
        }

        public void DeepCopyFrom(UserInfo other)
        {
            this.userId = other.userId;
            this.userName = other.userName;
            this.createTimeS = other.createTimeS;
            this.lastLoginTimeS = other.lastLoginTimeS;
        }

        #endregion auto
    }
}