using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class SceneRoomInfo : IIsDifferent<SceneRoomInfo>
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
        public long messageSeq;

        public static SceneRoomInfo Ensure(SceneRoomInfo? p)
        {
            if (p == null)
            {
                p = new SceneRoomInfo();
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
        }

        public bool IsDifferent(SceneRoomInfo other)
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
            if (this.messageSeq != other.messageSeq)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(SceneRoomInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
            this.title = other.title;
            this.desc = other.desc;
            this.messageSeq = other.messageSeq;
        }
    }
}
