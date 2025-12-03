using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_TournamentRankPlayerInfo
    {
        [Key(0)]
        public TournamentRankPlayerInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_TournamentRankPlayerInfo
    {
    }
}
