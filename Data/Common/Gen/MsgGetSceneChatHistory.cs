using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetSceneChatHistory
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long lastMessageId;
    }
}
