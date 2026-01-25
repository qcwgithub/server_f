using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class MsgLogin_Db : IIsDifferent_Db<MsgLogin>
    {
        #region auto

        [BsonIgnoreIfNull]
        public string version;
        [BsonIgnoreIfNull]
        public string platform;
        [BsonIgnoreIfNull]
        public string channel;
        [BsonIgnoreIfNull]
        public string channelUserId;
        [BsonIgnoreIfNull]
        public string verifyData;
        [BsonIgnoreIfNull]
        public long? userId;
        [BsonIgnoreIfNull]
        public string token;
        [BsonIgnoreIfNull]
        public string deviceUid;
        [BsonIgnoreIfNull]
        public Dictionary<string, string> dict;

        public bool DeepCopyFrom(MsgLogin other)
        {
            bool empty = true;

            this.version = XInfoHelper_Db.Copy_string(other.version);
            if (this.version != null)
            {
                empty = false;
            }

            this.platform = XInfoHelper_Db.Copy_string(other.platform);
            if (this.platform != null)
            {
                empty = false;
            }

            this.channel = XInfoHelper_Db.Copy_string(other.channel);
            if (this.channel != null)
            {
                empty = false;
            }

            this.channelUserId = XInfoHelper_Db.Copy_string(other.channelUserId);
            if (this.channelUserId != null)
            {
                empty = false;
            }

            this.verifyData = XInfoHelper_Db.Copy_string(other.verifyData);
            if (this.verifyData != null)
            {
                empty = false;
            }

            this.userId = XInfoHelper_Db.Copy_long(other.userId);
            if (this.userId != null)
            {
                empty = false;
            }

            this.token = XInfoHelper_Db.Copy_string(other.token);
            if (this.token != null)
            {
                empty = false;
            }

            this.deviceUid = XInfoHelper_Db.Copy_string(other.deviceUid);
            if (this.deviceUid != null)
            {
                empty = false;
            }

            this.dict = XInfoHelper_Db.Copy_DictValue(other.dict);
            if (this.dict != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}