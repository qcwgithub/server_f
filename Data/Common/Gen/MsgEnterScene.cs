using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgEnterScene
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long lastSeq;
    }
}
