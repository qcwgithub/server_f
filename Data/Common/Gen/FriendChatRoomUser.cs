using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class FriendChatRoomUser : IIsDifferent<FriendChatRoomUser>
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long joinTimeS;

        public static FriendChatRoomUser Ensure(FriendChatRoomUser? p)
        {
            if (p == null)
            {
                p = new FriendChatRoomUser();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
        }

        public bool IsDifferent(FriendChatRoomUser other)
        {
            if (this.userId != other.userId)
            {
                return true;
            }
            if (this.joinTimeS != other.joinTimeS)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(FriendChatRoomUser other)
        {
            this.userId = other.userId;
            this.joinTimeS = other.joinTimeS;
        }
    }
}
