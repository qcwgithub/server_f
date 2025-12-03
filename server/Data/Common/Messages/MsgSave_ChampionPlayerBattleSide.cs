using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ChampionPlayerBattleSide = Data.PlayerBattleSide;
    using ChampionPlayerBattleSide_Db = Data.PlayerBattleSide_Db;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ChampionPlayerBattleSide
    {
        [Key(0)]
        public ChampionPlayerBattleSide info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ChampionPlayerBattleSide
    {
    }
}
