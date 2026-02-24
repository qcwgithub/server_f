using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAckFriendChatReadSeq1
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public long readSeq;
    }
}
