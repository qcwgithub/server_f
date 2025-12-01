using UnityEngine;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    /*
    清空Redis能否正常 | YES，是运行时数据
    */
    public class LockRedis : ServerScript<BaseServer>
    {
        public IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        ////
        public async Task<string> LockForLoadFromDBToRedis(string lockKey, log4net.ILog logger)
        {
            return await RedisUtils.LockOnce(this.GetDb(), lockKey, 40, logger);
        }
        public async Task UnlockForLoadFromDBToRedis(string lockKey, string lockValue)
        {
            await RedisUtils.Unlock(this.GetDb(), lockKey, lockValue);
        }
        
        public async Task<string> LockOrder(string orderId, log4net.ILog logger)
        {
            string key = LockKey.Order(orderId);
            return await RedisUtils.Lock(this.GetDb(), key, 10, 10, 200, logger);
        }

        public async Task UnlockOrder(string orderId, string lockValue)
        {
            string key = LockKey.Order(orderId);
            await RedisUtils.Unlock(this.GetDb(), key, lockValue);
        }

        public async Task<string> LockAccount(string channel, string channelUserId, log4net.ILog logger)
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