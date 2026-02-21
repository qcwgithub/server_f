using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class PrivateRoomInfo : IIsDifferent<PrivateRoomInfo>
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long createTimeS;
        [Key(2)]
        public long messageId;
        [Key(3)]
        public List<RoomParticipant> participants;

        public static PrivateRoomInfo Ensure(PrivateRoomInfo? p)
        {
            if (p == null)
            {
                p = new PrivateRoomInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.participants == null)
            {
                this.participants = new List<RoomParticipant>();
            }
            for (int i = 0; i < this.participants.Count; i++)
            {
                this.participants[i] = RoomParticipant.Ensure(this.participants[i]);
            }
        }

        public bool IsDifferent(PrivateRoomInfo other)
        {
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.messageId != other.messageId)
            {
                return true;
            }
            if (this.participants.IsDifferent_ListClass(other.participants))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(PrivateRoomInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
            this.messageId = other.messageId;
            this.participants.DeepCopyFrom_ListClass(other.participants);
        }
    }
}
