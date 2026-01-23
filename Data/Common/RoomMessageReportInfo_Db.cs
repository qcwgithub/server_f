using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class RoomMessageReportInfo_Db : IIsDifferent_Db<RoomMessageReportInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public long? reportUserId;
        [BsonIgnoreIfNull]
        public long? targetUserId;
        [BsonIgnoreIfNull]
        public long? roomId;
        [BsonIgnoreIfNull]
        public long? messageId;
        [BsonIgnoreIfNull]
        public RoomMessageReportReason reason;
        [BsonIgnoreIfNull]
        public long? timeS;

        public bool DeepCopyFrom(RoomMessageReportInfo other)
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

            this.roomId = XInfoHelper_Db.Copy_long(other.roomId);
            if (this.roomId != null)
            {
                empty = false;
            }

            this.messageId = XInfoHelper_Db.Copy_long(other.messageId);
            if (this.messageId != null)
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