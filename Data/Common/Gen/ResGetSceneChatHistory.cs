using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResGetSceneChatHistory
    {
        [Key(0)]
        public List<ChatMessage> messages;
    }
}
