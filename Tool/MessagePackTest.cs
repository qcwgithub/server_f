using Data;
using MessagePack;

namespace Tool
{
    [MessagePackObject]
    public sealed class ChatMessageOld
    {
        [Key(0)]
        public long messageId;
        // [Key(1)]
        // public long roomId;
        // [Key(2)]
        // public long senderId;
        // [Key(3)]
        // public string? senderName;
        // [Key(4)]
        // public string? senderAvatar;

        // [Key(5)]
        // public ChatMessageType type;
        // [Key(6)]
        // public string? content;
    }

    [MessagePackObject]
    public sealed class ChatMessageMiddle
    {
        [Key(0)]
        public long messageId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public long senderId;
        [Key(3)]
        public string? senderName;
        [Key(4)]
        public string? senderAvatar;

        [Key(5)]
        public ChatMessageType type;
        [Key(6)]
        public string? content;

        [Key(7)]
        public long timestamp;
        [Key(8)]
        public long? replyTo; // message id
    }

    [MessagePackObject]
    public sealed class ChatMessageNew
    {
        [Key(0)]
        public long messageId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public long senderId;
        [Key(3)]
        public string? senderName;
        [Key(4)]
        public string? senderAvatar;

        [Key(5)]
        public ChatMessageType type;
        [Key(6)]
        public string? content;

        [Key(7)]
        public long timestamp;
        [Key(8)]
        public long? replyTo; // message id

        [Key(9)]
        public int newValue;
    }

    public static class MessagePackTest
    {
        static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception();
            }
        }

        public static void Test()
        {
            // single
            {
                var middle = new ChatMessageMiddle();
                middle.messageId = 9999;
                middle.roomId = 333;

                var middleBytes = MessagePackSerializer.Serialize(middle);
                var old = MessagePackSerializer.Deserialize<ChatMessageOld>(middleBytes);
                Assert(old.messageId == middle.messageId);

                var new_ = MessagePackSerializer.Deserialize<ChatMessageNew>(middleBytes);
                Assert(new_.messageId == middle.messageId);
            }

            // list
            {
                var middle = new List<ChatMessageMiddle>();
                middle.Add(new ChatMessageMiddle
                {
                    messageId = 9999,
                    roomId = 333
                });

                var middleBytes = MessagePackSerializer.Serialize(middle);
                var old = MessagePackSerializer.Deserialize<List<ChatMessageOld>>(middleBytes);
                Assert(old.Count == middle.Count && old[0].messageId == middle[0].messageId);

                var new_ = MessagePackSerializer.Deserialize<List<ChatMessageNew>>(middleBytes);
                Assert(new_.Count == middle.Count && new_[0].messageId == middle[0].messageId);
            }
        }
    }
}