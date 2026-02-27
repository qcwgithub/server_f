using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class FriendChatRoomUser_Db : IIsDifferent_Db<FriendChatRoomUser>
    {
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public long? joinTimeS;

        public bool DeepCopyFrom(FriendChatRoomUser other)
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
