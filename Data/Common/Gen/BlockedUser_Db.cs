using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class BlockedUser_Db : IIsDifferent_Db<BlockedUser>
    {
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public long? timeS;

        public bool DeepCopyFrom(BlockedUser other)
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

            return !empty;
        }
    }
}
