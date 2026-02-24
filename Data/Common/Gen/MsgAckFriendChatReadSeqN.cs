using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgAckFriendChatReadSeqN
    {
        [Key(0)]
        public Dictionary<long, long> roomIdToReadSeqs;
    }
}
