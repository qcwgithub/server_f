using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgARemoveFriend
    {
        [Key(0)]
        public long friendUserId;
        [Key(1)]
        public RemoveFriendReason reason;
    }
}
