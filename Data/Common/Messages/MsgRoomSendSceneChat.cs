using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomSendSceneChat
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long userId;
        [Key(2)]
        public ChatMessageType type;
        [Key(3)]
        public string? content;
        [Key(4)]
        public long? replyTo; // message id
        [Key(5)]
        public string userName;
        [Key(6)]
        public int avatarIndex;
        [Key(7)]
        public long clientMessageId;
        [Key(8)]
        public ChatMessageImageContent? imageContent;
    }

    [MessagePackObject]
    public class ResRoomSendSceneChat
    {

    }
}