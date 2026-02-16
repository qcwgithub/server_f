using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class FriendInfo : IIsDifferent<FriendInfo>
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long timeS;

        public static FriendInfo Ensure(FriendInfo? p)
        {
            if (p == null)
            {
                p = new FriendInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(FriendInfo other)
        {
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.timeS != other.timeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(FriendInfo other)
        {
            this.userId = other.userId;
            this.timeS = other.timeS;
        }
    }
}
