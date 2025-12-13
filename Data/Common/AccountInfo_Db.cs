using System.Collections.Generic;
using MessagePack;
using System.Numerics;
using MongoDB.Bson.Serialization.Attributes;

namespace Data
{
    public class AccountInfo_Db : IIsDifferent_Db<AccountInfo>
    {
        #region auto

        [BsonIgnoreIfNull]
        public int? isPlaceholder;
        [BsonIgnoreIfNull]
        public string platform;
        [BsonIgnoreIfNull]
        public string channel;
        [BsonIgnoreIfNull]
        public string channelUserId;
        [BsonIgnoreIfNull]
        public List<long> userIds;
        [BsonIgnoreIfNull]
        public long? createTimeS;
        [BsonIgnoreIfNull]
        public bool? block;
        [BsonIgnoreIfNull]
        public long? unblockTime;
        [BsonIgnoreIfNull]
        public string blockPrompt;
        [BsonIgnoreIfNull]
        public string blockOrUnblockReason;
        [BsonIgnoreIfNull]
        public long? lastLoginUserId;

        public bool DeepCopyFrom(AccountInfo other)
        {
            bool empty = true;

            this.isPlaceholder = ProfileHelper_Db.Copy_int(other.isPlaceholder);
            if (this.isPlaceholder != null)
            {
                empty = false;
            }

            this.platform = ProfileHelper_Db.Copy_string(other.platform);
            if (this.platform != null)
            {
                empty = false;
            }

            this.channel = ProfileHelper_Db.Copy_string(other.channel);
            if (this.channel != null)
            {
                empty = false;
            }

            this.channelUserId = ProfileHelper_Db.Copy_string(other.channelUserId);
            if (this.channelUserId != null)
            {
                empty = false;
            }

            this.userIds = ProfileHelper_Db.Copy_ListValue(other.userIds);
            if (this.userIds != null)
            {
                empty = false;
            }

            this.createTimeS = ProfileHelper_Db.Copy_long(other.createTimeS);
            if (this.createTimeS != null)
            {
                empty = false;
            }

            this.block = ProfileHelper_Db.Copy_bool(other.block);
            if (this.block != null)
            {
                empty = false;
            }

            this.unblockTime = ProfileHelper_Db.Copy_long(other.unblockTime);
            if (this.unblockTime != null)
            {
                empty = false;
            }

            this.blockPrompt = ProfileHelper_Db.Copy_string(other.blockPrompt);
            if (this.blockPrompt != null)
            {
                empty = false;
            }

            this.blockOrUnblockReason = ProfileHelper_Db.Copy_string(other.blockOrUnblockReason);
            if (this.blockOrUnblockReason != null)
            {
                empty = false;
            }

            this.lastLoginUserId = ProfileHelper_Db.Copy_long(other.lastLoginUserId);
            if (this.lastLoginUserId != null)
            {
                empty = false;
            }

            return !empty;
        }

        #endregion auto
    }
}