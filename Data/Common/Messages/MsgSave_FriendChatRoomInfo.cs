using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_FriendChatRoomInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public FriendChatRoomInfoNullable roomInfoNullable;
        [Key(2)]
        public FriendChatRoomInfo? roomInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_FriendChatRoomInfo
    {
    }
}