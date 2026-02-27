using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSetFriendChatReadSeq
    {
        [Key(0)]
        public long readSeq;
    }
}
