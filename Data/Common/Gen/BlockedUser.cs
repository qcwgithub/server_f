using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class BlockedUser : IIsDifferent<BlockedUser>
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long timeS;

        public static BlockedUser Ensure(BlockedUser? p)
        {
            if (p == null)
            {
                p = new BlockedUser();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(BlockedUser other)
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

        public void DeepCopyFrom(BlockedUser other)
        {
            this.userId = other.userId;
            this.timeS = other.timeS;
        }
    }
}
