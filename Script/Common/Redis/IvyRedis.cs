using System.Threading.Tasks;
using StackExchange.Redis;
using System;
using Data;

namespace Script
{
    public class IvyRedis : ServerScript<GroupServer>
    {
        public IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// access token

        public async Task<RedisValueWithExpiry> GetFacebookLoginInfo(string uid)
        {
            return await this.GetDb().StringGetWithExpiryAsync(IvyKey.FacebookAccountInfo(uid));
        }

        public async Task SaveFacebookLoginInfo(string uid, int aliveTimeDay)
        {
            await this.GetDb().StringSetAsync(IvyKey.FacebookAccountInfo(uid), uid, TimeSpan.FromDays(aliveTimeDay));
        }

        public async Task<RedisValueWithExpiry> GetGoogleLoginInfo(string uid)
        {
            return await this.GetDb().StringGetWithExpiryAsync(IvyKey.GoogleAccountInfo(uid));
        }

        public async Task SaveGoogleLoginInfo(string uid, int aliveTimeDay)
        {
            await this.GetDb().StringSetAsync(IvyKey.GoogleAccountInfo(uid), uid, TimeSpan.FromDays(aliveTimeDay));
        }
    }
}