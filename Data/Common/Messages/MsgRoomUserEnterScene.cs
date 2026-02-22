using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomUserEnterScene
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public int gatewayServiceId;
        [Key(3)]
        public long lastSeq;
    }

    [MessagePackObject]
    public class ResRoomUserEnterScene
    {
        [Key(0)]
        public List<ChatMessage> recentMessages;
    }
}