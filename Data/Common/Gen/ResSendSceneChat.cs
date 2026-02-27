using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSendSceneChat
    {
        [Key(0)]
        public ChatMessage message;
    }
}
