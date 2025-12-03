using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_AccountInfo_by_channelUserId
    {
        [Key(0)]
        public string channelUserId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_AccountInfo_by_channelUserId
    {
        [Key(0)]
        public AccountInfo result;
    }
}
