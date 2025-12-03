using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_GroupSignInInfo
    {
        [Key(0)]
        public GroupSignInInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_GroupSignInInfo
    {
    }
}
