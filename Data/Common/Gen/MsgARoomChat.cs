using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgARoomChat
    {
        [Key(0)]
        public ChatMessage message;
    }
}
