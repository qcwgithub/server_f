using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ChatMessage
    {
        [Key(0)]
        public long seq;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public long senderId;
        [Key(3)]
        public string senderName;
        [Key(4)]
        public string senderAvatar;
        [Key(5)]
        public ChatMessageType type;
        [Key(6)]
        public string content;
        [Key(7)]
        public long timestamp;
        [Key(8)]
        public long replyTo;
        [Key(9)]
        public int senderAvatarIndex;
        [Key(10)]
        public long clientMessageId;
        [Key(11)]
        public ChatMessageStatus status;
        [Key(12)]
        public ChatMessageImageContent? imageContent;
        [Key(13)]
        public long messageId;
    }
}
