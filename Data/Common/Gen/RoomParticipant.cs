using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class RoomParticipant : IIsDifferent<RoomParticipant>
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long joinTimeS;

        public static RoomParticipant Ensure(RoomParticipant? p)
        {
            if (p == null)
            {
                p = new RoomParticipant();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(RoomParticipant other)
        {
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.joinTimeS != other.joinTimeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(RoomParticipant other)
        {
            this.userId = other.userId;
            this.joinTimeS = other.joinTimeS;
        }
    }
}
