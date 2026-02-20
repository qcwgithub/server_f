using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAChatMessage
    {
        [Key(0)]
        public ChatMessage message;
    }
}
