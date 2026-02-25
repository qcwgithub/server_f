using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerCreateFriendChatRoom
    {
        [Key(0)]
        public List<long> userIds;
    }

    [MessagePackObject]
    public class ResRoomManagerCreateFriendChatRoom
    {
        [Key(0)]
        public FriendChatRoomInfo friendChatRoomInfo;
    }
}