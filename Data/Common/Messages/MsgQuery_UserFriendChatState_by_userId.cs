using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UserFriendChatState_by_userId
    {
        [Key(0)]
        public long userId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UserFriendChatState_by_userId
    {
        [Key(0)]
        public UserFriendChatState? result;
    }
}
