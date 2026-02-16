using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAReceiveFriendRequest
    {
        [Key(0)]
        public UserBriefInfo fromUserBriefInfo;
        [Key(1)]
        public IncomingFriendRequest req;
    }
}
