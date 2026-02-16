using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserBriefInfo : ICanBePlaceholder
    {
        [Key(0)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int isPlaceholder;
        public bool IsPlaceholder() => this.isPlaceholder == 1;
        public void SetIsPlaceholder() => this.isPlaceholder = 1;
        [Key(1)]
        public long userId;
        [Key(2)]
        public string userName;
        [Key(3)]
        public int avatarIndex;

        public static UserBriefInfo Ensure(UserBriefInfo? p)
        {
            if (p == null)
            {
                p = new UserBriefInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.userName == null)
            {
                this.userName = string.Empty;
            }
        }
    }
}
