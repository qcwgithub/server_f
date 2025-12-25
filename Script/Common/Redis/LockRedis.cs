using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Linq;


namespace Script
{
    /*
    清空Redis能否正常 | YES，是运行时数据
    */
    public class LockRedis : ServerScript
    {
        public LockRedis(Server server) : base(server)
        {
            
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        ////
        public async Task<string?> LockForLoadFromDBToRedis(string lockKey, log4net.ILog logger)
        {
            return await RedisUtils.LockOnce(this.GetDb(), lockKey, 40, logger);
        }
        public async Task UnlockForLoadFromDBToRedis(string lockKey, string lockValue)
        {
            await RedisUtils.Unlock(this.GetDb(), lockKey, lockValue);
        }

        public async Task<string?> LockAccount(string channel, string channelUserId, log4net.ILog logger)
        {
            string key = LockKey.Account(channel, channelUserId);
            return await RedisUtils.LockOnce(this.GetDb(), key, 60, logger);
        }

        public async Task UnlockAccount(string channel, string channelUserId, string lockValue)
        {
            string key = LockKey.Account(channel, channelUserId);
            await RedisUtils.Unlock(this.GetDb(), key, lockValue);
        }

        public async Task<bool> IsForbid(string channelUserId)
        {
            return await this.GetDb().KeyExistsAsync(GAAAKey.ForbidAccount(channelUserId));
        }
    }
}