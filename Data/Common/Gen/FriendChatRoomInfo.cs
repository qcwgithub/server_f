using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class FriendChatRoomInfo : IIsDifferent<FriendChatRoomInfo>
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long createTimeS;
        [Key(2)]
        public long messageSeq;
        [Key(3)]
        public List<FriendChatRoomUser> users;

        public static FriendChatRoomInfo Ensure(FriendChatRoomInfo? p)
        {
            if (p == null)
            {
                p = new FriendChatRoomInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.users == null)
            {
                this.users = new List<FriendChatRoomUser>();
            }
            for (int i = 0; i < this.users.Count; i++)
            {
                this.users[i] = FriendChatRoomUser.Ensure(this.users[i]);
            }
        }

        public bool IsDifferent(FriendChatRoomInfo other)
        {
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.messageSeq != other.messageSeq)
            {
                return true;
            }
            if (this.users.IsDifferent_ListClass(other.users))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(FriendChatRoomInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
            this.messageSeq = other.messageSeq;
            this.users.DeepCopyFrom_ListClass(other.users);
        }
    }
}
