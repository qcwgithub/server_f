using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using longid = System.Int64;

namespace Script
{
    public class UnionNamesRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.UnionNamesInitedFlag(serverId);

        /*
        public async Task<int> IncreaseAndGetNextUnionId()
        {
            RedisValue redisValue = await GetDb().StringIncrementAsync(UnionGlobalKey.NextUnionId());
            int nextUnionId = RedisUtils.ParseInt(redisValue);
            return nextUnionId;
        }
        */

        string Key(int serverId, string name) => NameKey.UnionName(serverId, SCUtils.Base64Encode(name));
        string SKey(int serverId, string shortName) => NameKey.UnionShortName(serverId, SCUtils.Base64Encode(shortName));

        public async Task AddManyNames(int serverId, Dictionary<longid, UnionInfo_name_shortName> dict)
        {
            var array = new KeyValuePair<RedisKey, RedisValue>[dict.Count * 2];
            int i = 0;

            foreach (var kv in dict)
            {
                array[i++] = new KeyValuePair<RedisKey, RedisValue>(
                    Key(serverId, kv.Value.name),
                    kv.Key
                );

                array[i++] = new KeyValuePair<RedisKey, RedisValue>(
                    SKey(serverId, kv.Value.shortName),
                    kv.Key
                );
            }

            await GetDb().StringSetAsync(array);
        }

        public async Task AddUnionName(int serverId, string name, longid unionId)
        {
            await GetDb().StringSetAsync(Key(serverId, name), unionId);
        }

        public async Task DeleteUnionName(int serverId, string name)
        {
            await GetDb().KeyDeleteAsync(Key(serverId, name));
        }

        public async Task<longid> FindUnionIdByName(int serverId, string name)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key(serverId, name));
            return RedisUtils.ParseLongId(redisValue);
        }

        public async Task AddUnionShortName(int serverId, string shortName, longid unionId)
        {
            await GetDb().StringSetAsync(SKey(serverId, shortName), unionId);
        }

        public async Task DeleteUnionShortName(int serverId, string shortName)
        {
            await GetDb().KeyDeleteAsync(SKey(serverId, shortName));
        }

        public async Task<longid> FindUnionIdByShortName(int serverId, string shortName)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(SKey(serverId, shortName));
            return RedisUtils.ParseLongId(redisValue);
        }

        bool CanUse(RedisValue redisValue, longid allowUnionId)
        {
            longid u = RedisUtils.ParseLongId(redisValue);
            return (u == 0 || u == allowUnionId);
        }

        public async Task<ECode> CheckCanUseName(int serverId, string name, string shortName, longid allowUnionId)
        {
            bool b1 = !string.IsNullOrEmpty(name);
            bool b2 = !string.IsNullOrEmpty(shortName);
            if (b1 && b2)
            {
                RedisValue[] redisValues = await GetDb().StringGetAsync(new RedisKey[] { Key(serverId, name), SKey(serverId, shortName) });
                if (!this.CanUse(redisValues[0], allowUnionId))
                {
                    return ECode.UnionNameExisted;
                }
                if (!this.CanUse(redisValues[1], allowUnionId))
                {
                    return ECode.UnionShortNameExisted;
                }
            }
            else if (b1)
            {
                RedisValue redisValue = await GetDb().StringGetAsync(Key(serverId, name));
                if (!this.CanUse(redisValue, allowUnionId))
                {
                    return ECode.UnionNameExisted;
                }
            }
            else if (b2)
            {
                RedisValue redisValue = await GetDb().StringGetAsync(SKey(serverId, shortName));
                if (!this.CanUse(redisValue, allowUnionId))
                {
                    return ECode.UnionShortNameExisted;
                }
            }

            return ECode.Success;
        }
    }

}