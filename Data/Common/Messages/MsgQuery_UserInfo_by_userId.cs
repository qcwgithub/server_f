using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UserInfo_by_userId
    {
        [Key(0)]
        public long userId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UserInfo_by_userId
    {
        [Key(0)]
        public UserInfo? result;
    }
}
