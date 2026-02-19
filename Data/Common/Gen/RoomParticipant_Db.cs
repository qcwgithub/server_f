using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class RoomParticipant_Db : IIsDifferent_Db<RoomParticipant>
    {
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public long? joinTimeS;

        public bool DeepCopyFrom(RoomParticipant other)
        {
            bool empty = true;

            this.userId = XInfoHelper_Db.Copy_long(other.userId);
            if (this.userId != null)
            {
                empty = false;
            }

            this.joinTimeS = XInfoHelper_Db.Copy_long(other.joinTimeS);
            if (this.joinTimeS != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
