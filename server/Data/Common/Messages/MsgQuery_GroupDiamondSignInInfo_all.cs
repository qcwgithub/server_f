using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_GroupDiamondSignInInfo_all
    {
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_GroupDiamondSignInInfo_all
    {
        [Key(0)]
        public GroupDiamondSignInInfo result;
    }
}
