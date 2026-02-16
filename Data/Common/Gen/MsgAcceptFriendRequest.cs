using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAcceptFriendRequest
    {
        [Key(0)]
        public long fromUserId;
    }
}
