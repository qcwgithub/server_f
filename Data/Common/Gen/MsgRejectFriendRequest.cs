using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRejectFriendRequest
    {
        [Key(0)]
        public long fromUserId;
    }
}
