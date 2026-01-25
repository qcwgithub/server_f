using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResGetRoomChatHistory
    {
        [Key(0)]
        public List<ChatMessage> history;
    }
}
