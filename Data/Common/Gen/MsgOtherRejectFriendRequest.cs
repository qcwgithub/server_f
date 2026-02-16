using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgOtherRejectFriendRequest
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long otherUserId;
    }

    [MessagePackObject]
    public class ResOtherRejectFriendRequest
    {
    }
}
