using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class PrivateRoomInfo_Db : IIsDifferent_Db<PrivateRoomInfo>
    {
        [BsonIgnoreIfNull]
        public long? roomId;
        [BsonIgnoreIfNull]
        public long? createTimeS;
        [BsonIgnoreIfNull]
        public long? messageId;
        [BsonIgnoreIfNull]
        public List<RoomParticipant_Db> participants;

        public bool DeepCopyFrom(PrivateRoomInfo other)
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

            this.messageId = XInfoHelper_Db.Copy_long(other.messageId);
            if (this.messageId != null)
            {
                empty = false;
            }

            this.participants = XInfoHelper_Db.Copy_ListClass<RoomParticipant_Db, RoomParticipant>(other.participants);
            if (this.participants != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
