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
        public long? seq;
        [BsonIgnoreIfNull]
        public List<PrivateRoomUser_Db> users;

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

            this.seq = XInfoHelper_Db.Copy_long(other.seq);
            if (this.seq != null)
            {
                empty = false;
            }

            this.users = XInfoHelper_Db.Copy_ListClass<PrivateRoomUser_Db, PrivateRoomUser>(other.users);
            if (this.users != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
