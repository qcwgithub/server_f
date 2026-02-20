using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResEnterScene
    {
        [Key(0)]
        public List<ChatMessage> recentMessages;
    }
}
