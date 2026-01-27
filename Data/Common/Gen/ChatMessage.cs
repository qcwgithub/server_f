using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ChatMessage
    {
        [Key(0)]
        public long messageId;
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
    }
}
