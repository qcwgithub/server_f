using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerCreatePrivateRoom
    {
        [Key(0)]
        public List<long> participants;
    }

    [MessagePackObject]
    public class ResRoomManagerCreatePrivateRoom
    {
        [Key(0)]
        public FriendChatInfo friendChatInfo;
    }
}