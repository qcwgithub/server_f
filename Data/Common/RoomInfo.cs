using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class RoomInfo
    {
        #region auto

        [Key(0)]
        public long roomId;

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
            return false;
        }

        public void DeepCopyFrom(RoomInfo other)
        {
            this.roomId = other.roomId;
        }

        #endregion auto
    }
}