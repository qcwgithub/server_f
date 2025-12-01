using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using longid = System.Int64;

namespace Script
{
    public abstract class ApexSeatsRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.ApexInitedFlag();
        protected abstract string Key(int season, int grade);

        // 返回值：移除了几个
        public async Task<long> RemoveASeat(int season, int grade, longid groupId)
        {
            RedisValue redisValue = await this.GetDb().ListRemoveAsync(this.Key(season, grade), groupId, 1/* ! */);
            return RedisUtils.ParseLong(redisValue);
        }

        public async Task<long> RemoveSeats(int season, int grade, longid groupId, int count)
        {
            RedisValue redisValue = await this.GetDb().ListRemoveAsync(this.Key(season, grade), groupId, count);
            return RedisUtils.ParseLong(redisValue);
        }

        public async Task<longid> TakeASeat(int season, int grade)
        {
            RedisValue redisValue = await this.GetDb().ListLeftPopAsync(this.Key(season, grade));
            return RedisUtils.ParseLongId(redisValue);
        }

        public async Task AddSeat(int season, int grade, longid seat)
        {
            await this.GetDb().ListRightPushAsync(this.Key(season, grade), seat);
        }

        public async Task AddSeats(int season, int grade, List<longid> seats)
        {
            if (seats.Count == 0)
            {
                return;
            }

            await this.GetDb().ListRightPushAsync(this.Key(season, grade), seats.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task Clear(int season, int grade)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(season, grade));
        }
    }

    public class ApexNormalSeatsRedis : ApexSeatsRedis
    {
        protected override string Key(int season, int grade)
        {
            return ApexKey.Seats(season, grade);
        }
    }

    public class ApexRobotSeatsRedis : ApexSeatsRedis
    {
        protected override string Key(int season, int grade)
        {
            return ApexKey.RobotSeats(season, grade);
        }
    }
}