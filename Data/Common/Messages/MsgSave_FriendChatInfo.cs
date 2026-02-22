using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_FriendChatInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public FriendChatInfoInfoNullable privateRoomInfoNullable;
        [Key(2)]
        public FriendChatInfo? friendChatInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_FriendChatInfo
    {
    }
}