using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_MatchGamePlayerInfo
    {
        [Key(0)]
        public MatchGamePlayerInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_MatchGamePlayerInfo
    {
    }
}
