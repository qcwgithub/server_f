using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Script
{
    public class ChampionSeatsRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ChampionInitedFlag();

        public async Task<int> TakeASeat()
        {
            RedisValue redisValue = await this.GetDb().ListLeftPopAsync(ChampionKey.Seats());
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task AddSeats(List<int> seats)
        {
            await this.GetDb().ListRightPushAsync(ChampionKey.Seats(), seats.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task Clear()
        {
            await this.GetDb().KeyDeleteAsync(ChampionKey.Seats());
        }
    }
}