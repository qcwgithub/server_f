using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgReceiveChatMessage
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public ChatMessage message;
    }

    [MessagePackObject]
    public class ResReceiveChatMessage
    {
    }
}
