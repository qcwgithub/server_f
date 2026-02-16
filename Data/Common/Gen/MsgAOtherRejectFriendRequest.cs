using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAOtherRejectFriendRequest
    {
        [Key(0)]
        public long otherUserId;
    }
}
