using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResReceiveFriendChatMessages
    {
        [Key(0)]
        public List<ChatMessage> messages;
        [Key(1)]
        public bool hasMore;
    }
}
