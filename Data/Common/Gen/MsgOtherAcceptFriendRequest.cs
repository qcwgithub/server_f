using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgOtherAcceptFriendRequest
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long otherUserId;
        [Key(2)]
        public long privateRoomId;
    }

    [MessagePackObject]
    public class ResOtherAcceptFriendRequest
    {
    }
}
