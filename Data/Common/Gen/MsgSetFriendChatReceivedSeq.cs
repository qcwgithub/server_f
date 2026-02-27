using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetFriendChatReceivedSeq
    {
        [Key(0)]
        public long friendUserId;
        [Key(1)]
        public long receivedSeq;
    }
}
