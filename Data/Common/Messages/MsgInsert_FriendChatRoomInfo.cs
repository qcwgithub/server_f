using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_FriendChatRoomInfo
    {
        [Key(0)]
        public FriendChatRoomInfo roomInfo;
    }

    [MessagePackObject]
    public class ResInsert_FriendChatRoomInfo
    {

    }
}