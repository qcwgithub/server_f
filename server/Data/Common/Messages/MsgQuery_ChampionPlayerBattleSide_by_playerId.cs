using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ChampionPlayerBattleSide = Data.PlayerBattleSide;
    using ChampionPlayerBattleSide_Db = Data.PlayerBattleSide_Db;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ChampionPlayerBattleSide_by_playerId
    {
        [Key(0)]
        public longid playerId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ChampionPlayerBattleSide_by_playerId
    {
        [Key(0)]
        public ChampionPlayerBattleSide result;
    }
}
