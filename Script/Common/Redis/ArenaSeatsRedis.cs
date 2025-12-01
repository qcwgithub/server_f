using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Script
{
    public class ArenaSeatsRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ArenaInitedFlag();

        public async Task<int> TakeASeat()
        {
            RedisValue redisValue = await this.GetDb().ListLeftPopAsync(ArenaKey.Seats());
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task AddSeats(List<int> seats)
        {
            await this.GetDb().ListRightPushAsync(ArenaKey.Seats(), seats.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task Clear()
        {
            await this.GetDb().KeyDeleteAsync(ArenaKey.Seats());
        }
    }
}