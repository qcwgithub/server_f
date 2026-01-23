using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class RoomMessageReportInfo
    {
        #region auto

        [Key(0)]
        public long reportUserId;
        [Key(1)]
        public long targetUserId;
        [Key(2)]
        public long roomId;
        [Key(3)]
        public long messageId;
        [Key(4)]
        public RoomMessageReportReason reason;
        [Key(5)]
        public long timeS;

        public static RoomMessageReportInfo Ensure(RoomMessageReportInfo? p)
        {
            if (p == null)
            {
                p = new RoomMessageReportInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(RoomMessageReportInfo other)
        {
            if (this.reportUserId != other.reportUserId)
            {
                return true;
            }
            if (this.targetUserId != other.targetUserId)
            {
                return true;
            }
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.messageId != other.messageId)
            {
                return true;
            }
            if (this.reason != other.reason)
            {
                return true;
            }
            if (this.timeS != other.timeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(RoomMessageReportInfo other)
        {
            this.reportUserId = other.reportUserId;
            this.targetUserId = other.targetUserId;
            this.roomId = other.roomId;
            this.messageId = other.messageId;
            this.reason = other.reason;
            this.timeS = other.timeS;
        }

        #endregion auto
    }
}