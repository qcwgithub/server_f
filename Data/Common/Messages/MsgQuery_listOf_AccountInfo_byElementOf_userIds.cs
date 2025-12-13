using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_AccountInfo_byElementOf_userIds
    {
        [Key(0)]
        public long ele_userIds;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_AccountInfo_byElementOf_userIds
    {
        [Key(0)]
        public List<AccountInfo> result;
    }
}
