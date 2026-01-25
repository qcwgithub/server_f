using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserReportInfo
    {
        #region auto

        [Key(0)]
        public long reportUserId;
        [Key(1)]
        public long targetUserId;
        [Key(2)]
        public UserReportReason reason;
        [Key(3)]
        public long timeS;

        public static UserReportInfo Ensure(UserReportInfo? p)
        {
            if (p == null)
            {
                p = new UserReportInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(UserReportInfo other)
        {
            if (this.reportUserId != other.reportUserId)
            {
                return true;
            }
            if (this.targetUserId != other.targetUserId)
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

        public void DeepCopyFrom(UserReportInfo other)
        {
            this.reportUserId = other.reportUserId;
            this.targetUserId = other.targetUserId;
            this.reason = other.reason;
            this.timeS = other.timeS;
        }

        #endregion auto
    }
}