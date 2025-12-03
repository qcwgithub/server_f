using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_RankingListLike_by_memberId
    {
        [Key(0)]
        public longid start_memberId;
        [Key(1)]
        public longid end_memberId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_RankingListLike_by_memberId
    {
        [Key(0)]
        public List<RankingListLike> result;
    }
}
