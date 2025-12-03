using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ApexRobotInfo_all
    {
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ApexRobotInfo_all
    {
        [Key(0)]
        public ApexRobotInfo result;
    }
}
