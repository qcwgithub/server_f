using System;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using longid = System.Int64;

namespace Script
{
    public class ChampionPlayerMiscRedis : ServiceScript<NormalServer, StatelessService>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        //// Competitors
        //// 元素是 rank
        public async Task<List<int>> GetCompetitors(longid playerId)
        {
            return await RedisUtils.GetFromJson<List<int>>(this.GetDb(), ChampionKey.Player.Competitors(playerId));
        }
        public async Task ClearCompetitors(longid playerId)
        {
            await GetDb().KeyDeleteAsync(ChampionKey.Player.Competitors(playerId));
        }
        public async void SetCompetitors(longid playerId, List<int> list)
        {
            await RedisUtils.SaveAsJson(this.GetDb(), ChampionKey.Player.Competitors(playerId), list, TimeSpan.FromDays(1));
        }

        //// Records
        const int ALIVE_TIME = 86400 * 3; // 3 天
        public async Task<List<ChampionFightRecord>> GetRecords(longid playerId, bool removeExpire)
        {
            List<ChampionFightRecord> list = await RedisUtils.GetListJson<ChampionFightRecord>(this.GetDb(), ChampionKey.Player.Records(playerId));
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

                this.service.logger.DebugFormat("Champion.GetRecords playerId {0} expireCount {1}", playerId, expireCount);

                if (expireCount > 0)
                {
                    if (this.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                    {
                        await GetDb().ListLeftPopAsync(ChampionKey.Player.Records(playerId), expireCount);
                    }
                    else
                    {
                        var tasks = new List<Task>();
                        for (int i = 0; i < expireCount; i++)
                        {
                            tasks.Add(this.GetDb().ListLeftPopAsync(ChampionKey.Player.Records(playerId)));
                        }
                        await Task.WhenAll(tasks);
                    }
                }
            }

            return list;
        }

        // public async Task<ChampionFightRecord> GetRecords(longid playerId, long rowId)
        // {
        //     List<ChampionFightRecord> listRecords = await GetRecords(playerId);
        //     ChampionFightRecord findRecord = listRecords.Find(r => r.rowId == rowId);
        //     return findRecord;
        // }
        public async Task ClearRecords(longid playerId)
        {
            await GetDb().KeyDeleteAsync(ChampionKey.Player.Records(playerId));
        }

        public async Task<int> GetRecordLosesTime(longid playerId)
        {
            var rValue = await this.GetDb().StringGetAsync(ChampionKey.Player.RecordLosesTime(playerId));
            return RedisUtils.ParseInt(rValue);
        }

        public async Task AddRecord(longid playerId, ChampionFightRecord record, int countLimit)
        {
            if (countLimit <= 0)
            {
                return;
            }

            int listCount = (int)await GetDb().ListLengthAsync(ChampionKey.Player.Records(playerId));
            if (listCount > countLimit)
            {
                if (this.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                {
                    await GetDb().ListLeftPopAsync(ChampionKey.Player.Records(playerId), listCount - countLimit);
                }
                else
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < listCount - countLimit; i++)
                    {
                        tasks.Add(this.GetDb().ListLeftPopAsync(ChampionKey.Player.Records(playerId)));
                    }
                    await Task.WhenAll(tasks);
                }
            }

            await GetDb().ListRightPushAsync(ChampionKey.Player.Records(playerId), JsonUtils.stringify(record));
            // await GetDb().StringIncrementAsync(ChampionKey.Player.RecordsDirty(playerId));
            if (record.result == -1)
            {
                await GetDb().StringSetAsync(ChampionKey.Player.RecordLosesTime(playerId), record.timeS.ToString());
            }

        }

        // public void PopRecord(longid playerId)
        // {
        //     GetDb().ListLeftPopAsync(ChampionKey.Player.Records(playerId)).Forget();
        //     GetDb().StringIncrementAsync(ChampionKey.Player.RecordsDirty(playerId)).Forget();
        // }

        public async void SaveRecord(longid playerId, int index, ChampionFightRecord record)
        {
            await GetDb().ListSetByIndexAsync(ChampionKey.Player.Records(playerId), index, JsonUtils.stringify(record));
            // GetDb().StringIncrementAsync(ChampionKey.Player.RecordsDirty(playerId)).Forget();
        }
    }
}