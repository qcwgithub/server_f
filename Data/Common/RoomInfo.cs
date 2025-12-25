using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class RoomInfo
    {
        #region auto

        [Key(0)]
        public long roomId;
        [Key(1)]
        public long createTimeS;

        public static RoomInfo Ensure(RoomInfo? p)
        {
            if (p == null)
            {
                p = new RoomInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(RoomInfo other)
        {
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(RoomInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
        }

        #endregion auto
    }
}