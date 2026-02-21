using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class SceneInfo : IIsDifferent<SceneInfo>
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long createTimeS;
        [Key(2)]
        public string title;
        [Key(3)]
        public string desc;
        [Key(4)]
        public long messageId;
        [Key(5)]
        public List<RoomParticipant> participants;

        public static SceneInfo Ensure(SceneInfo? p)
        {
            if (p == null)
            {
                p = new SceneInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.title == null)
            {
                this.title = string.Empty;
            }
            if (this.desc == null)
            {
                this.desc = string.Empty;
            }
            if (this.participants == null)
            {
                this.participants = new List<RoomParticipant>();
            }
            for (int i = 0; i < this.participants.Count; i++)
            {
                this.participants[i] = RoomParticipant.Ensure(this.participants[i]);
            }
        }

        public bool IsDifferent(SceneInfo other)
        {
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.title != other.title)
            {
                return true;
            }
            if (this.desc != other.desc)
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

        public void DeepCopyFrom(SceneInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
            this.title = other.title;
            this.desc = other.desc;
            this.messageId = other.messageId;
            this.participants.DeepCopyFrom_ListClass(other.participants);
        }
    }
}
