using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetSceneChatHistory
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long beforeSeq;
        [Key(2)]
        public long afterSeq;
        [Key(3)]
        public int count;
    }
}
