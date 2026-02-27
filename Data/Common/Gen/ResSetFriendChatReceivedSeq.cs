using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSetFriendChatReceivedSeq
    {
        [Key(0)]
        public long receivedSeq;
    }
}
