using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserFriendChatState : ICanBePlaceholder, IIsDifferent<UserFriendChatState>
    {
        [Key(0)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int isPlaceholder;
        public bool IsPlaceholder() => this.isPlaceholder == 1;
        public void SetIsPlaceholder() => this.isPlaceholder = 1;
        [Key(1)]
        public long userId;
        [Key(2)]
        public Dictionary<long, UserFriendChatStateRoom> roomDict;

        public static UserFriendChatState Ensure(UserFriendChatState? p)
        {
            if (p == null)
            {
                p = new UserFriendChatState();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.roomDict == null)
            {
                this.roomDict = new Dictionary<long, UserFriendChatStateRoom>();
            }
            foreach (var kv in this.roomDict)
            {
                UserFriendChatStateRoom.Ensure(kv.Value);
            }
        }

        public bool IsDifferent(UserFriendChatState other)
        {
            if (this.isPlaceholder != other.isPlaceholder)
            {
                return true;
            }
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.roomDict.IsDifferent_DictClass(other.roomDict))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(UserFriendChatState other)
        {
            this.isPlaceholder = other.isPlaceholder;
            this.userId = other.userId;
            this.roomDict.DeepCopyFrom_DictClass(other.roomDict);
        }
    }
}
