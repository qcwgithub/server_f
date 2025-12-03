using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ArenaGroupRobotInfo_by_groupType_groupId
    {
        [Key(0)]
        public int groupType;
        [Key(1)]
        public int groupId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ArenaGroupRobotInfo_by_groupType_groupId
    {
        [Key(0)]
        public ArenaGroupRobotInfo result;
    }
}
