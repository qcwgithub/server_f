using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class ResLogin_Db : IIsDifferent_Db<ResLogin>
    {
        #region auto

        [BsonIgnoreIfNull]
        public bool? isNewUser;
        [BsonIgnoreIfNull]
        public UserInfo_Db userInfo;
        [BsonIgnoreIfNull]
        public bool? kickOther;

        public bool DeepCopyFrom(ResLogin other)
        {
            bool empty = true;

            this.isNewUser = XInfoHelper_Db.Copy_bool(other.isNewUser);
            if (this.isNewUser != null)
            {
                empty = false;
            }

            this.userInfo = XInfoHelper_Db.Copy_Class<UserInfo_Db, UserInfo>(other.userInfo);
            if (this.userInfo != null)
            {
                empty = false;
            }

            this.kickOther = XInfoHelper_Db.Copy_bool(other.kickOther);
            if (this.kickOther != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}