using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class UserInfo_Db : IIsDifferent_Db<UserInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public string userName;
        [BsonIgnoreIfNull]
        public long? createTimeS;
        [BsonIgnoreIfNull]
        public long? lastLoginTimeS;

        public bool DeepCopyFrom(UserInfo other)
        {
            bool empty = true;

            this.userId = ProfileHelper_Db.Copy_long(other.userId);
            if (this.userId != null)
            {
                empty = false;
            }

            this.userName = ProfileHelper_Db.Copy_string(other.userName);
            if (this.userName != null)
            {
                empty = false;
            }

            this.createTimeS = ProfileHelper_Db.Copy_long(other.createTimeS);
            if (this.createTimeS != null)
            {
                empty = false;
            }

            this.lastLoginTimeS = ProfileHelper_Db.Copy_long(other.lastLoginTimeS);
            if (this.lastLoginTimeS != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}