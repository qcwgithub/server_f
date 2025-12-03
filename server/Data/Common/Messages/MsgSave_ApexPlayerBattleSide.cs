using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ApexPlayerBattleSide = Data.PlayerBattleSide;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ApexPlayerBattleSide
    {
        [Key(0)]
        public ApexPlayerBattleSide info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ApexPlayerBattleSide
    {
    }
}
