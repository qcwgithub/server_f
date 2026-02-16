using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class OutgoingFriendRequest_Db : IIsDifferent_Db<OutgoingFriendRequest>
    {
        [BsonIgnoreIfNull]
        public long? toUserId;
        [BsonIgnoreIfNull]
        public long? timeS;
        [BsonIgnoreIfNull]
        public string say;
        [BsonIgnoreIfNull]
        public FriendRequestResult result;

        public bool DeepCopyFrom(OutgoingFriendRequest other)
        {
            bool empty = true;

            this.toUserId = XInfoHelper_Db.Copy_long(other.toUserId);
            if (this.toUserId != null)
            {
                empty = false;
            }

            this.timeS = XInfoHelper_Db.Copy_long(other.timeS);
            if (this.timeS != null)
            {
                empty = false;
            }

            this.say = XInfoHelper_Db.Copy_string(other.say);
            if (this.say != null)
            {
                empty = false;
            }

            this.result = XInfoHelper_Db.Copy_Enum(other.result);
            empty = false;

            return !empty;
        }
    }
}
