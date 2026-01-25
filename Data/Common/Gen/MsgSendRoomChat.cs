using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSendRoomChat
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public ChatMessageType chatMessageType;
        [Key(2)]
        public string content;
    }
}
