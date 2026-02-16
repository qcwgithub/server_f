using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class OutgoingFriendRequest : IIsDifferent<OutgoingFriendRequest>
    {
        [Key(0)]
        public long toUserId;
        [Key(1)]
        public long timeS;
        [Key(2)]
        public string say;
        [Key(3)]
        public FriendRequestResult result;

        public static OutgoingFriendRequest Ensure(OutgoingFriendRequest? p)
        {
            if (p == null)
            {
                p = new OutgoingFriendRequest();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.say == null)
            {
                this.say = string.Empty;
            }
        }

        public bool IsDifferent(OutgoingFriendRequest other)
        {
            if (this.toUserId != other.toUserId)
            {
                return true;
            }
            if (this.timeS != other.timeS)
            {
                return true;
            }
            if (this.say != other.say)
            {
                return true;
            }
            if (this.result != other.result)
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(OutgoingFriendRequest other)
        {
            this.toUserId = other.toUserId;
            this.timeS = other.timeS;
            this.say = other.say;
            this.result = other.result;
        }
    }
}
