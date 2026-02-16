using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAOtherAcceptFriendRequest
    {
        [Key(0)]
        public long otherUserId;
        [Key(1)]
        public FriendInfo friendInfo;
    }
}
