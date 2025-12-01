using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using longid = System.Int64;

namespace Script
{
    public class UnionMapSeatsRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.UnionMapSeatsInitedFlag(serverId);
        public static RedisKey Key(longid unionId) => WorldMapKey.UnionMapSeats(unionId);

        public async Task<int?> TakeASeat(longid unionId)
        {
            RedisValue redisValue = await this.GetDb().ListLeftPopAsync(Key(unionId));
            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }

            return RedisUtils.ParseInt(redisValue);
        }

        public async Task<long> RemoveSeat(longid unionId, int seat)
        {
            return await this.GetDb().ListRemoveAsync(Key(unionId), new RedisValue(seat.ToString()), 1);
        }

        public async Task AddSeats(longid unionId, List<int> seats)
        {
            await this.GetDb().ListRightPushAsync(Key(unionId), seats.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task AddSeat_Front(longid unionId, int seat)
        {
            await this.GetDb().ListLeftPushAsync(Key(unionId), seat);
        }

        public async Task Clear(longid unionId)
        {
            await this.GetDb().KeyDeleteAsync(Key(unionId));
        }
    }
}