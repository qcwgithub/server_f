using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class Profile : IIsDifferent<Profile>
    {
        #region auto

        [Key(0)]
        public long userId;
        [Key(1)]
        public string userName;
        [Key(2)]
        public long createTime;
        [Key(3)]
        public long lastLoginTimeS;

        public static Profile Ensure(Profile? p)
        {
            if (p == null)
            {
                p = new Profile();
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

        public bool IsDifferent(Profile other)
        {
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.userName != other.userName)
            {
                return true;
            }
            if (this.createTime != other.createTime)
            {
                return true;
            }
            if (this.lastLoginTimeS != other.lastLoginTimeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(Profile other)
        {
            this.userId = other.userId;
            this.userName = other.userName;
            this.createTime = other.createTime;
            this.lastLoginTimeS = other.lastLoginTimeS;
        }

        #endregion auto
    }
}