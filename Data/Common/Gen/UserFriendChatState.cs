using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserFriendChatState
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public long messageId;
    }
}
