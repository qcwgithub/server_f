using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public class PlayerPotentialMailsRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public string Key(longid playerId) => PlayerKey.PotentialMails(playerId);
        protected override string waitKey => GGlobalKey.MailInitedFlag();

        public async Task Add(longid playerId, longid mailId)
        {
            await this.GetDb().SetAddAsync(this.Key(playerId), mailId);
        }

        public async Task AddMany(HashSet<longid> playerIds, longid mailId)
        {
            var tasks = new List<Task>();
            foreach (longid playerId in playerIds)
            {
                tasks.Add(this.GetDb().SetAddAsync(this.Key(playerId), mailId));
            }
            await Task.WhenAll(tasks);
        }

        public async Task Remove(longid playerId, longid mailId)
        {
            await this.GetDb().SetRemoveAsync(this.Key(playerId), mailId);
        }

        public async Task RemoveMany(longid playerId, List<longid> mailIds)
        {
            await this.GetDb().SetRemoveAsync(this.Key(playerId), mailIds.Select(_ => new RedisValue(_.ToString())).ToArray());
        }

        public async Task<HashSet<longid>> Get(longid playerId)
        {
            RedisValue[] redisValues = await this.GetDb().SetMembersAsync(this.Key(playerId));
            return redisValues.Select(_ => RedisUtils.ParseLongId(_)).ToHashSet();
        }
    }
}