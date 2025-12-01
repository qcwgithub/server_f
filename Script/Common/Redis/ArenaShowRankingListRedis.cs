using System;
using System.Threading.Tasks;
using longid = System.Int64;

namespace Script
{
    public class ArenaShowRankingListRedis : ArenaRankingListRedis
    {
        public override string Key(int groupType, int groupId)
        {
            string key = ArenaKey.ShowRankingList(groupType, groupId);
            return key;
        }

        public async Task<int> CalucatePageCount(int groupType, int groupId, int? length, int PER_PAGE)
        {
            if (length == null)
            {
                length = await this.GetLength(groupType, groupId);
            }
            int pageCount = length.Value / PER_PAGE;
            if (length % PER_PAGE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
    }

    public class TempArenaShowRankingListRedis : ArenaRankingListRedis
    {
        public override string Key(int groupType, int groupId)
        {
            string key = TemporaryKey.ToTemporaryKey(ArenaKey.ShowRankingList(groupType, groupId), "forSumUp");
            return key;
        }
    }
}