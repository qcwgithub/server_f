using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MessageReportInfo : IIsDifferent<MessageReportInfo>
    {
        [Key(0)]
        public long reportUserId;
        [Key(1)]
        public long targetUserId;
        [Key(2)]
        public long roomId;
        [Key(3)]
        public long seq;
        [Key(4)]
        public MessageReportReason reason;
        [Key(5)]
        public long timeS;

        public static MessageReportInfo Ensure(MessageReportInfo? p)
        {
            if (p == null)
            {
                p = new MessageReportInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(MessageReportInfo other)
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
            if (this.seq != other.seq)
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

        public void DeepCopyFrom(MessageReportInfo other)
        {
            this.reportUserId = other.reportUserId;
            this.targetUserId = other.targetUserId;
            this.roomId = other.roomId;
            this.seq = other.seq;
            this.reason = other.reason;
            this.timeS = other.timeS;
        }
    }
}
