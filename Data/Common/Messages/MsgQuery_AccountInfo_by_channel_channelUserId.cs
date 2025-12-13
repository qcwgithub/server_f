using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_AccountInfo_by_channel_channelUserId
    {
        [Key(0)]
        public string channel;
        [Key(1)]
        public string channelUserId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_AccountInfo_by_channel_channelUserId
    {
        [Key(0)]
        public AccountInfo result;
    }
}
