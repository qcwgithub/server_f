using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class AccountInfo : ICanBePlaceholder
    {
        #region auto

        [Key(0)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int isPlaceholder;
        public bool IsPlaceholder() => this.isPlaceholder == 1;
        public void SetIsPlaceholder() => this.isPlaceholder = 1;
        [Key(1)]
        public string platform;
        [Key(2)]
        public string channel;
        [Key(3)]
        public string channelUserId;
        [Key(4)]
        public List<long> userIds;
        [Key(5)]
        public long createTimeS;
        [Key(6)]
        public bool block;
        [Key(7)]
        public long unblockTime;
        [Key(8)]
        public string blockPrompt;
        [Key(9)]
        public string blockOrUnblockReason;
        [Key(10)]
        public long lastLoginUserId;

        public static AccountInfo Ensure(AccountInfo? p)
        {
            if (p == null)
            {
                p = new AccountInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.platform == null)
            {
                this.platform = string.Empty;
            }
            if (this.channel == null)
            {
                this.channel = string.Empty;
            }
            if (this.channelUserId == null)
            {
                this.channelUserId = string.Empty;
            }
            if (this.userIds == null)
            {
                this.userIds = new List<long>();
            }
            if (this.blockPrompt == null)
            {
                this.blockPrompt = string.Empty;
            }
            if (this.blockOrUnblockReason == null)
            {
                this.blockOrUnblockReason = string.Empty;
            }
        }

        public bool IsDifferent(AccountInfo other)
        {
            if (this.isPlaceholder != other.isPlaceholder)
            {
                return true;
            }
            if (this.platform != other.platform)
            {
                return true;
            }
            if (this.channel != other.channel)
            {
                return true;
            }
            if (this.channelUserId != other.channelUserId)
            {
                return true;
            }
            if (this.userIds.IsDifferent_ListValue(other.userIds))
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.block != other.block)
            {
                return true;
            }
            if (this.unblockTime != other.unblockTime)
            {
                return true;
            }
            if (this.blockPrompt != other.blockPrompt)
            {
                return true;
            }
            if (this.blockOrUnblockReason != other.blockOrUnblockReason)
            {
                return true;
            }
            if (this.lastLoginUserId != other.lastLoginUserId)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(AccountInfo other)
        {
            this.isPlaceholder = other.isPlaceholder;
            this.platform = other.platform;
            this.channel = other.channel;
            this.channelUserId = other.channelUserId;
            this.userIds.DeepCopyFrom_ListValue(other.userIds);
            this.createTimeS = other.createTimeS;
            this.block = other.block;
            this.unblockTime = other.unblockTime;
            this.blockPrompt = other.blockPrompt;
            this.blockOrUnblockReason = other.blockOrUnblockReason;
            this.lastLoginUserId = other.lastLoginUserId;
        }

        #endregion auto

        public static int ToTaskQueueHash(string channelUserId)
        {
            return channelUserId.GetHashCode();
        }
    }
}