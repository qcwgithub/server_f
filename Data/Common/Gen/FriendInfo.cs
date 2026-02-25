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
        [Key(3)]
        public long readSeq;
        [Key(4)]
        public long receivedSeq;

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
            if (this.readSeq != other.readSeq)
            {
                return true;
            }
            if (this.receivedSeq != other.receivedSeq)
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
            this.readSeq = other.readSeq;
            this.receivedSeq = other.receivedSeq;
        }
    }
}
