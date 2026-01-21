using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgForward
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long[]? userIds;
        [Key(2)]
        public MsgType innerMsgType;
        [Key(3)]
        public byte[] innerMsgBytes;
    }
}