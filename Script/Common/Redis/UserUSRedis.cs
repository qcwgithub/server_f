using StackExchange.Redis;

namespace Script
{
    public class UserUSRedis: ServerScript
    {
        public UserUSRedis(Server server) : base(server)
        {
            
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public string Key(long playerId) => UserKey.USId(playerId);

        public async Task SetUSId(long userId, int playerServiceId, int secondsToLive)
        {
            await GetDb().StringSetAsync(UserKey.USId(userId), playerServiceId, TimeSpan.FromSeconds(secondsToLive));
        }

        public async Task<int> GetUSId(long userId)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(UserKey.USId(userId));
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task<List<int>> GetMany(List<long> userIds)
        {
            RedisValue[] redisValues = await GetDb().StringGetAsync(userIds.Select(_ => new RedisKey(UserKey.USId(_))).ToArray());
            return redisValues.Select(_ => RedisUtils.ParseInt(_)).ToList();
        }

        public async void DeletePSId(long playerId)
        {
            await GetDb().KeyDeleteAsync(UserKey.USId(playerId));
        }
    }
}