using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetRoomChatHistory
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long lastMessageId;
    }
}
