using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class RoomInfo_Db : IIsDifferent_Db<RoomInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public long? roomId;

        public bool DeepCopyFrom(RoomInfo other)
        {
            bool empty = true;

            this.roomId = XInfoHelper_Db.Copy_long(other.roomId);
            if (this.roomId != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}