using longid = System.Int64;
namespace Script
{
    public class ArenaMatchRankingListRedis : ArenaRankingListRedis
    {
        public override string Key(int groupType, int groupId)
        {
            string key = ArenaKey.MatchRankingList(groupType, groupId);
            return key;
        }
    }

    public class TempArenaMatchRankingListRedis : ArenaRankingListRedis
    {
        public override string Key(int groupType, int groupId)
        {
            string key = TemporaryKey.ToTemporaryKey(ArenaKey.MatchRankingList(groupType, groupId), "forSumUp");
            return key;
        }
    }
}