using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResAcceptFriendRequest
    {
        [Key(0)]
        public FriendInfo friendInfo;
    }
}
