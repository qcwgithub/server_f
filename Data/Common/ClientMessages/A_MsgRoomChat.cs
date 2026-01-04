using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class A_MsgRoomChat
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long userId;
        [Key(2)]
        public ChatMessageType chatMessageType;
        [Key(3)]
        public string content;
    }
}