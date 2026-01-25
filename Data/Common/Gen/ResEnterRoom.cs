using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResEnterRoom
    {
        [Key(0)]
        public List<ChatMessage> recentMessages;
    }
}
