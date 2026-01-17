using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgGetRoomChatHistory
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long lastMessageId;
    }

    [MessagePackObject]
    public class ResGetRoomChatHistory
    {
        [Key(0)]
        public List<ChatMessage> history;
    }
}