using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class FriendInfo_Db : IIsDifferent_Db<FriendInfo>
    {
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public long? timeS;
        [BsonIgnoreIfNull]
        public long? roomId;

        public bool DeepCopyFrom(FriendInfo other)
        {
            bool empty = true;

            this.userId = XInfoHelper_Db.Copy_long(other.userId);
            if (this.userId != null)
            {
                empty = false;
            }

            this.timeS = XInfoHelper_Db.Copy_long(other.timeS);
            if (this.timeS != null)
            {
                empty = false;
            }

            this.roomId = XInfoHelper_Db.Copy_long(other.roomId);
            if (this.roomId != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
