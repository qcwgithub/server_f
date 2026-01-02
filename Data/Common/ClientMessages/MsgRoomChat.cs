using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomChat
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public ChatMessageType chatMessageType;
        [Key(2)]
        public string content;
    }

    [MessagePackObject]
    public class ResRoomChat
    {

    }
}