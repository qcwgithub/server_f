using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class IncomingFriendRequest : IIsDifferent<IncomingFriendRequest>
    {
        [Key(0)]
        public long fromUserId;
        [Key(1)]
        public long timeS;
        [Key(2)]
        public string say;
        [Key(3)]
        public FriendRequestResult result;

        public static IncomingFriendRequest Ensure(IncomingFriendRequest? p)
        {
            if (p == null)
            {
                p = new IncomingFriendRequest();
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

        public bool IsDifferent(IncomingFriendRequest other)
        {
            if (this.fromUserId != other.fromUserId)
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

        public void DeepCopyFrom(IncomingFriendRequest other)
        {
            this.fromUserId = other.fromUserId;
            this.timeS = other.timeS;
            this.say = other.say;
            this.result = other.result;
        }
    }
}
