using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResReceiveFriendChatMessages
    {
        [Key(0)]
        public Dictionary<long, ChatMessageList> messageListDict;
        [Key(1)]
        public bool hasMore;
    }
}
