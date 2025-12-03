using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_GroupRadarPassInfo
    {
        [Key(0)]
        public GroupRadarPassInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_GroupRadarPassInfo
    {
    }
}
