using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResLogin
    {
        [Key(0)]
        public bool isNewUser;
        [Key(1)]
        public UserInfo userInfo;
        [Key(2)]
        public bool kickOther;

        public static ResLogin Ensure(ResLogin? p)
        {
            if (p == null)
            {
                p = new ResLogin();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            this.userInfo = UserInfo.Ensure(this.userInfo);
        }

        public bool IsDifferent(ResLogin other)
        {
            if (this.isNewUser != other.isNewUser)
            {
                return true;
            }
            if (this.userInfo.IsDifferent(other.userInfo))
            {
                return true;
            }
            if (this.kickOther != other.kickOther)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(ResLogin other)
        {
            this.isNewUser = other.isNewUser;
            this.userInfo.DeepCopyFrom(other.userInfo);
            this.kickOther = other.kickOther;
        }
    }
}
