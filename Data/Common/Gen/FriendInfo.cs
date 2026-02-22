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
        [Key(2)]
        public long roomId;

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
            if (this.roomId != other.roomId)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(FriendInfo other)
        {
            this.userId = other.userId;
            this.timeS = other.timeS;
            this.roomId = other.roomId;
        }
    }
}
