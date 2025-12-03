using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_WorldMapPlayerInfo
    {
        [Key(0)]
        public WorldMapPlayerInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_WorldMapPlayerInfo
    {
    }
}
