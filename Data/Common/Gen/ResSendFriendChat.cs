using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSendFriendChat
    {
        [Key(0)]
        public ChatMessage message;
    }
}
