using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserBriefInfo : ICanBePlaceholder, IIsDifferent<UserBriefInfo>
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

        public bool IsDifferent(UserBriefInfo other)
        {
            if (this.isPlaceholder != other.isPlaceholder)
            {
                return true;
            }
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.userName != other.userName)
            {
                return true;
            }
            if (this.avatarIndex != other.avatarIndex)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(UserBriefInfo other)
        {
            this.isPlaceholder = other.isPlaceholder;
            this.userId = other.userId;
            this.userName = other.userName;
            this.avatarIndex = other.avatarIndex;
        }
    }
}
