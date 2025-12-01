using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using longid = System.Int64;

namespace Script
{
    public class TournamentSeatsRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.TournamentInitedFlag();

        public async Task<longid> TakeASeat(int season, int grade)
        {
            RedisValue redisValue = await this.GetDb().ListLeftPopAsync(TournamentKey.Seats(season, grade));
            return RedisUtils.ParseLongId(redisValue);
        }

        public async Task AddSeats(int season, int grade, List<longid> seats)
        {
            await this.GetDb().ListRightPushAsync(TournamentKey.Seats(season, grade), seats.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task Clear(int season, int grade)
        {
            await this.GetDb().KeyDeleteAsync(TournamentKey.Seats(season, grade));
        }
    }
}