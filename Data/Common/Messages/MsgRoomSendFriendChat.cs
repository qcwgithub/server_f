using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomSendFriendChat
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
        public long clientSeq;
        [Key(8)]
        public ChatMessageImageContent? imageContent;
    }

    [MessagePackObject]
    public class ResRoomSendFriendChat
    {
        [Key(0)]
        public ChatMessage message;
    }
}