using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserFriendChatStateRoom : IIsDifferent<UserFriendChatStateRoom>
    {
        [Key(0)]
        public long maxSeq;
        [Key(1)]
        public long readSeq;
        [Key(2)]
        public long unreadCount;

        public static UserFriendChatStateRoom Ensure(UserFriendChatStateRoom? p)
        {
            if (p == null)
            {
                p = new UserFriendChatStateRoom();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(UserFriendChatStateRoom other)
        {
            if (this.maxSeq != other.maxSeq)
            {
                return true;
            }
            if (this.readSeq != other.readSeq)
            {
                return true;
            }
            if (this.unreadCount != other.unreadCount)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(UserFriendChatStateRoom other)
        {
            this.maxSeq = other.maxSeq;
            this.readSeq = other.readSeq;
            this.unreadCount = other.unreadCount;
        }
    }
}
