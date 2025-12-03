using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ApexPlayerBattleSide = Data.PlayerBattleSide;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ApexPlayerBattleSide_by_playerId
    {
        [Key(0)]
        public longid playerId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ApexPlayerBattleSide_by_playerId
    {
        [Key(0)]
        public ApexPlayerBattleSide result;
    }
}
