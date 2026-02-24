using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_FriendChatInfo
    {
        [Key(0)]
        public FriendChatInfo privateRoomInfo;
    }

    [MessagePackObject]
    public class ResInsert_FriendChatInfo
    {

    }
}