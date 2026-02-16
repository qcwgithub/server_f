using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgOtherRemoveFriend
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long otherUserId;
    }

    [MessagePackObject]
    public class ResOtherRemoveFriend
    {
    }
}
