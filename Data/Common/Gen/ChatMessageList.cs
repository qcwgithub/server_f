using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ChatMessageList
    {
        [Key(0)]
        public List<ChatMessage> list;
    }
}
