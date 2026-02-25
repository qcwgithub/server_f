using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetFriendChatReadSeq
    {
        [Key(0)]
        public long friendUserId;
        [Key(1)]
        public long readSeq;
    }
}
