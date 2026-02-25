using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class SceneRoomInfo_Db : IIsDifferent_Db<SceneRoomInfo>
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
        public long? messageSeq;

        public bool DeepCopyFrom(SceneRoomInfo other)
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

            this.messageSeq = XInfoHelper_Db.Copy_long(other.messageSeq);
            if (this.messageSeq != null)
            {
                empty = false;
            }

            return !empty;
        }
    }
}
