using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomSendChat
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long userId;
        [Key(2)]
        public ChatMessageType type;
        [Key(3)]
        public string content;
        [Key(4)]
        public long? replyTo; // message id
    }

    [MessagePackObject]
    public class ResRoomSendChat
    {

    }
}