using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Script
{
    public class TournamentGradePlayerCountRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        string Key() => TournamentKey.GradePlayerCount();

        protected override string waitKey => GGlobalKey.TournamentInitedFlag();

        public async Task Increase(int grade, int count)
        {
            await this.GetDb().HashIncrementAsync(this.Key(), grade, count);
        }

        public async Task<int> GetPlayerCount(int grade)
        {
            RedisValue redisValue = await this.GetDb().HashGetAsync(this.Key(), grade);
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task Clear()
        {
            await this.GetDb().KeyDeleteAsync(this.Key());
        }

        //// 寄存一下
        public async Task<int> GetLastCheckIncreaseMaxGradeS()
        {
            RedisValue redisValue = await this.GetDb().StringGetAsync(TournamentKey.LastCheckIncreaseMaxGradeS());
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task SaveLastCheckIncreateMaxGradeS(int timeS)
        {
            await this.GetDb().StringSetAsync(TournamentKey.LastCheckIncreaseMaxGradeS(), new RedisValue(timeS.ToString()));
        }
    }
}