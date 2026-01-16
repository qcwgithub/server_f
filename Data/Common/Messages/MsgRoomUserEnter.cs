using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomUserEnter
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public int gatewayServiceId;
        [Key(3)]
        public long lastMessageId;
    }

    [MessagePackObject]
    public class ResRoomUserEnter
    {
        [Key(0)]
        public List<ChatMessage> recentMessages;
    }
}