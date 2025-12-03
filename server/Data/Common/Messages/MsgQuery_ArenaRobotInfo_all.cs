using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ArenaRobotInfo_all
    {
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ArenaRobotInfo_all
    {
        [Key(0)]
        public ArenaRobotInfo result;
    }
}
