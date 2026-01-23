using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class UserReportInfo_Db : IIsDifferent_Db<UserReportInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public long? reportUserId;
        [BsonIgnoreIfNull]
        public long? targetUserId;
        [BsonIgnoreIfNull]
        public UserReportReason reason;
        [BsonIgnoreIfNull]
        public long? timeS;

        public bool DeepCopyFrom(UserReportInfo other)
        {
            bool empty = true;

            this.reportUserId = XInfoHelper_Db.Copy_long(other.reportUserId);
            if (this.reportUserId != null)
            {
                empty = false;
            }

            this.targetUserId = XInfoHelper_Db.Copy_long(other.targetUserId);
            if (this.targetUserId != null)
            {
                empty = false;
            }

            this.reason = XInfoHelper_Db.Copy_Enum(other.reason);
            empty = false;

            this.timeS = XInfoHelper_Db.Copy_long(other.timeS);
            if (this.timeS != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}