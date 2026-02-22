using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class FriendChatInfo : IIsDifferent<FriendChatInfo>
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long createTimeS;
        [Key(2)]
        public long seq;
        [Key(3)]
        public List<PrivateRoomUser> users;

        public static FriendChatInfo Ensure(FriendChatInfo? p)
        {
            if (p == null)
            {
                p = new FriendChatInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.users == null)
            {
                this.users = new List<PrivateRoomUser>();
            }
            for (int i = 0; i < this.users.Count; i++)
            {
                this.users[i] = PrivateRoomUser.Ensure(this.users[i]);
            }
        }

        public bool IsDifferent(FriendChatInfo other)
        {
            if (this.roomId != other.roomId)
            {
                return true;
            }
            if (this.createTimeS != other.createTimeS)
            {
                return true;
            }
            if (this.seq != other.seq)
            {
                return true;
            }
            if (this.users.IsDifferent_ListClass(other.users))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(FriendChatInfo other)
        {
            this.roomId = other.roomId;
            this.createTimeS = other.createTimeS;
            this.seq = other.seq;
            this.users.DeepCopyFrom_ListClass(other.users);
        }
    }
}
