using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSendFriendRequest
    {
        [Key(0)]
        public long toUserId;
        [Key(1)]
        public string say;
    }
}
