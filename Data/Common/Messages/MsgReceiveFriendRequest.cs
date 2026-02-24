using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgReceiveFriendRequest
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long fromUserId;
        [Key(2)]
        public string say;
        [Key(3)]
        public UserBriefInfo fromUserBriefInfo;
    }

    [MessagePackObject]
    public class ResReceiveFriendRequest
    {
    }
}
