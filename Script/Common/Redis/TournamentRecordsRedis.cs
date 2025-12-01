using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using longid = System.Int64;
using StackExchange.Redis;

namespace Script
{
    public class TournamentRecordsRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        //// Records
        const int ALIVE_TIME = 86400 * 3; // 3 天
        public async Task<List<TournamentFightRecord>> GetRecords(longid playerId, bool removeExpire)
        {
            List<TournamentFightRecord> list = await RedisUtils.GetListJson<TournamentFightRecord>(this.GetDb(), TournamentKey.PlayerRecords(playerId));
            if (removeExpire && list != null && list.Count > 0)
            {
                int nowS = TimeUtils.GetTimeS();

                int expireCount = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (nowS - list[i].timeS > ALIVE_TIME)
                    {
                        expireCount++;

                        list.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        break;
                    }
                }

                if (expireCount > 0)
                {
                    if (this.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                    {
                        await GetDb().ListLeftPopAsync(TournamentKey.PlayerRecords(playerId), expireCount);
                    }
                    else
                    {
                        var tasks = new List<Task>();
                        for (int i = 0; i < expireCount; i++)
                        {
                            tasks.Add(this.GetDb().ListLeftPopAsync(TournamentKey.PlayerRecords(playerId)));
                        }
                        await Task.WhenAll(tasks);
                    }
                }
            }

            return list;
        }

        public async Task ClearRecords(longid playerId)
        {
            await GetDb().KeyDeleteAsync(TournamentKey.PlayerRecords(playerId));
        }

        public async Task AddRecord(longid playerId, TournamentFightRecord record, int countLimit)
        {
            if (countLimit <= 0)
            {
                return;
            }

            int listCount = (int)await GetDb().ListLengthAsync(TournamentKey.PlayerRecords(playerId));
            if (listCount > countLimit)
            {
                if (this.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                {
                    await GetDb().ListLeftPopAsync(TournamentKey.PlayerRecords(playerId), listCount - countLimit);
                }
                else
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < listCount - countLimit; i++)
                    {
                        tasks.Add(this.GetDb().ListLeftPopAsync(TournamentKey.PlayerRecords(playerId)));
                    }
                    await Task.WhenAll(tasks);
                }
            }

            await GetDb().ListRightPushAsync(TournamentKey.PlayerRecords(playerId), JsonUtils.stringify(record));
        }
    }
}