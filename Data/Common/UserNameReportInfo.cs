using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserNameReportInfo
    {
        #region auto

        [Key(0)]
        public long reportUserId;
        [Key(1)]
        public long targetUserId;
        [Key(2)]
        public UserNameReportReason reason;
        [Key(3)]
        public long timeS;
        [Key(4)]
        public string targetUserName;

        public static UserNameReportInfo Ensure(UserNameReportInfo? p)
        {
            if (p == null)
            {
                p = new UserNameReportInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.targetUserName == null)
            {
                this.targetUserName = string.Empty;
            }
        }

        public bool IsDifferent(UserNameReportInfo other)
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
            if (this.targetUserName != other.targetUserName)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(UserNameReportInfo other)
        {
            this.reportUserId = other.reportUserId;
            this.targetUserId = other.targetUserId;
            this.reason = other.reason;
            this.timeS = other.timeS;
            this.targetUserName = other.targetUserName;
        }

        #endregion auto
    }
}