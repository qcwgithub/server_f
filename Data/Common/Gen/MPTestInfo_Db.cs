using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class MPTestInfo_Db : IIsDifferent_Db<MPTestInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public int? intValue;
        [BsonIgnoreIfNull]
        public bool? boolValue;
        [BsonIgnoreIfNull]
        public long? longValue;
        [BsonIgnoreIfNull]
        public string stringValue;
        [BsonIgnoreIfNull]
        public List<int> listOfInt;
        [BsonIgnoreIfNull]
        public List<long> listOfLong;
        [BsonIgnoreIfNull]
        public List<string> listOfString;

        public bool DeepCopyFrom(MPTestInfo other)
        {
            bool empty = true;

            this.intValue = XInfoHelper_Db.Copy_int(other.intValue);
            if (this.intValue != null)
            {
                empty = false;
            }

            this.boolValue = XInfoHelper_Db.Copy_bool(other.boolValue);
            if (this.boolValue != null)
            {
                empty = false;
            }

            this.longValue = XInfoHelper_Db.Copy_long(other.longValue);
            if (this.longValue != null)
            {
                empty = false;
            }

            this.stringValue = XInfoHelper_Db.Copy_string(other.stringValue);
            if (this.stringValue != null)
            {
                empty = false;
            }

            this.listOfInt = XInfoHelper_Db.Copy_ListValue(other.listOfInt);
            if (this.listOfInt != null)
            {
                empty = false;
            }

            this.listOfLong = XInfoHelper_Db.Copy_ListValue(other.listOfLong);
            if (this.listOfLong != null)
            {
                empty = false;
            }

            this.listOfString = XInfoHelper_Db.Copy_ListValue(other.listOfString);
            if (this.listOfString != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}