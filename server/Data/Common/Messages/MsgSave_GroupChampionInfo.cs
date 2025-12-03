using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_GroupChampionInfo
    {
        [Key(0)]
        public GroupChampionInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_GroupChampionInfo
    {
    }
}
