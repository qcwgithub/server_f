using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class AccountInfo : ICanBePlaceholder
    {
        [Key(0)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int isPlaceholder;
        public bool IsPlaceholder() => this.isPlaceholder == 1;
        public void SetIsPlaceholder() => this.isPlaceholder = 1;

        [Key(1)]
        public List<long> userIds;
        [Key(2)]
        public string platform;
        [Key(3)]
        public string channel;
        [Key(4)]
        public string channelUserId;
        [Key(5)]
        public int createTimeS;

        [Key(6)]
        public bool block;
        [Key(7)]
        public long unblockTime;
        [Key(8)]
        public string blockPrompt;
        [Key(9)]
        public string blockOrUnblockReason; // 内部操作原因

        public static int ToTaskQueueHash(string channelUserId)
        {
            return channelUserId.GetHashCode();
        }

        // 最近一次登录的 playerId，用于登录时自动选择
        [Key(13)]
        public long lastLoginUserId;

        public void Ensure()
        {
            if (this.userIds == null)
            {
                this.userIds = new List<long>();
            }
        }
    }
}