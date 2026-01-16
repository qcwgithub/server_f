using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgEnterRoom
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long lastMessageId;
    }

    [MessagePackObject]
    public class ResEnterRoom
    {
        [Key(0)]
        public List<ChatMessage> recentMessages;
    }
}