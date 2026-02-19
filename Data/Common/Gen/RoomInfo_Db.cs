using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class RoomInfo_Db : IIsDifferent_Db<RoomInfo>
    {
        [BsonIgnoreIfNull]
        public long? roomId;
        [BsonIgnoreIfNull]
        public long? createTimeS;
        [BsonIgnoreIfNull]
        public string title;
        [BsonIgnoreIfNull]
        public string desc;
        [BsonIgnoreIfNull]
        public long? messageId;
        [BsonIgnoreIfNull]
        public RoomType roomType;
        [BsonIgnoreIfNull]
        public List<RoomParticipant_Db> participants;

        public bool DeepCopyFrom(RoomInfo other)
        {
            bool empty = true;

            this.roomId = XInfoHelper_Db.Copy_long(other.roomId);
            if (this.roomId != null)
            {
                empty = false;
            }

            this.createTimeS = XInfoHelper_Db.Copy_long(other.createTimeS);
            if (this.createTimeS != null)
            {
                empty = false;
            }

            this.title = XInfoHelper_Db.Copy_string(other.title);
            if (this.title != null)
            {
                empty = false;
            }

            this.desc = XInfoHelper_Db.Copy_string(other.desc);
            if (this.desc != null)
            {
                empty = false;
            }

            this.messageId = XInfoHelper_Db.Copy_long(other.messageId);
            if (this.messageId != null)
            {
                empty = false;
            }

            this.roomType = XInfoHelper_Db.Copy_Enum(other.roomType);
            empty = false;

            this.participants = XInfoHelper_Db.Copy_ListClass<RoomParticipant_Db, RoomParticipant>(other.participants);
            if (this.participants != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
