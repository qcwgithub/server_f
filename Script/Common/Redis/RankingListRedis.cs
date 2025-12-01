using Data;
using System.Collections.Generic;
using System.Numerics;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public class RankingListRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        static (RedisKey, ServiceType?) Key(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider)
        {
            return RankKey.RankData(twoServerId, config, provider);
        }

        static async Task<(RedisKey, ServiceType?)> KeyAsync(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProviderAsync providerAsync)
        {
            return await RankKey.RankDataAsync(twoServerId, config, providerAsync);
        }

        public override string WaitKey(int serverId) => GlobalKey.AllRankDataInitedFlag(serverId);

        public async Task<(RedisKey, bool)> Clear(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task AddManyRankData(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, Dictionary<longid, double> toAddData)
        {
            List<SortedSetEntry> entryData = new List<SortedSetEntry>();
            foreach (var kv in toAddData)
            {
                entryData.Add(new SortedSetEntry(kv.Key, kv.Value));
            }

            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            MyDebug.Assert(serviceType == null);
            await GetDb().SortedSetAddAsync(key, entryData.ToArray());
        }

        public async Task AddManyRankData(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, List<SortedSetEntry> toAddData)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            MyDebug.Assert(serviceType == null);
            await GetDb().SortedSetAddAsync(key, toAddData.ToArray());
        }

        public void SetNewScore(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid playerId, int score)
        {
            SetNewScoreInternal(twoServerId, config, provider, playerId, (double)score);
        }

        public void SetNewScore(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid playerId, double score)
        {
            SetNewScoreInternal(twoServerId, config, provider, playerId, score);
        }

        async void SetNewScoreInternal(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid playerId, double score)
        {
            double oldScore = await this.GetScore(twoServerId, config, provider, playerId);
            if (oldScore == score)
            {
                // 去除重复
                return;
            }

            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            MyDebug.Assert(serviceType == null);
            await GetDb().SortedSetAddAsync(key, playerId, TimeUtils.AddTimeFactor(score));
        }

        // 获取某个排名范围的数据
        public async Task<List<RankItemInfo>> GetRange(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, int startIndex, int endIndex, NormalService service)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            var list = new List<RankItemInfo>();

            if (serviceType == null)
            {
                SortedSetEntry[] entries = await GetDb().SortedSetRangeByRankWithScoresAsync(key, startIndex, endIndex, Order.Descending);
                if (entries.Length > 0)
                {
                    List<int> likes = await this.server.rankingListLikeRedis.GetMany(twoServerId, config.rankName, entries.Select(e => RedisUtils.ParseLongId(e.Element)).ToList());

                    for (int i = 0; i < entries.Length; i++)
                    {
                        var e = entries[i];
                        var info = new RankItemInfo();
                        info.id = RedisUtils.ParseLongId(e.Element);
                        info.rank = i + startIndex + 1;
                        info.score = TimeUtils.RemoveTimeFactor(e.Score);
                        info.like = likes[i];
                        list.Add(info);
                    }
                }
            }
            else
            {
                bool b = service.connectToOtherServiceDict.TryGetValue(serviceType.Value, out ConnectToOtherService connectToOtherService);
                if (!b)
                {
                    MyDebug.Assert(false);
                    return list;
                }

                var r = await connectToOtherService.SendAsync(MsgType._RankRange, new MsgRankRange { key = key, startIndex = startIndex, endIndex = endIndex });
                if (r.err == ECode.Success)
                {
                    var res = r.CastRes<ResRankRange>();
                    list.AddRange(res.list);
                }
            }

            return list;
        }

        public async Task<int> GetRank(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid playerIdOrUnionId)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            MyDebug.Assert(serviceType == null);

            long? r = await GetDb().SortedSetRankAsync(key, playerIdOrUnionId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        // 简化处理，这里得到的就是客户端显示的排名（已经+1处理）
        public async Task<RankItemInfo> GetOne(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid memberId /* playerId or unionId */, NormalService service)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            return await this.GetOne(twoServerId, config, key, serviceType, memberId, service);
        }

        public async Task<RankItemInfo> GetOne(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProviderAsync providerAsync, longid memberId /* playerId or unionId */, NormalService service)
        {
            (RedisKey key, ServiceType? serviceType) = await KeyAsync(twoServerId, config, providerAsync);
            return await this.GetOne(twoServerId, config, key, serviceType, memberId, service);
        }

        async Task<RankItemInfo> GetOne(stTwoServerId twoServerId, RankingListConfig config, string key, ServiceType? serviceType, longid memberId /* playerId or unionId */, NormalService service)
        {
            var info = new RankItemInfo();
            info.id = memberId;
            info.rank = RankUtils.INVALID_RANK;
            info.score = 0.0;
            info.like = 0;

            if (serviceType == null)
            {
                Task<long?> task1 = GetDb().SortedSetRankAsync(key, memberId, Order.Descending);
                Task<double?> task2 = GetDb().SortedSetScoreAsync(key, memberId);
                Task<int> task3 = this.server.rankingListLikeRedis.GetLike(twoServerId, config.rankName, memberId);
                await Task.WhenAll(task1, task2, task3);

                long? pRank = task1.Result;
                info.rank = pRank == null ? RankUtils.INVALID_RANK : RankUtils.FromIndex((int)pRank.Value);

                double? pScore = task2.Result;
                info.score = pScore == null ? 0.0 : TimeUtils.RemoveTimeFactor(pScore.Value);

                info.like = task3.Result;
            }
            else
            {
                bool b = service.connectToOtherServiceDict.TryGetValue(serviceType.Value, out ConnectToOtherService connectToOtherService);
                if (!b)
                {
                    MyDebug.Assert(false);
                    return info;
                }

                var r = await connectToOtherService.SendAsync(MsgType._RankGetOne, new MsgRankGetOne { key = key, memberId = memberId });
                if (r.err == ECode.Success)
                {
                    var res = r.CastRes<ResRankGetOne>();

                    long? pRank = res.rank;
                    info.rank = pRank == null ? RankUtils.INVALID_RANK : RankUtils.FromIndex((int)pRank.Value);

                    double? pScore = res.score;
                    info.score = pScore == null ? 0.0 : TimeUtils.RemoveTimeFactor(pScore.Value);

                    info.like = 0;
                }
            }

            info.rank = this.LimitRank(config, info.rank);

            return info;
        }

        public async Task<double> GetScore(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, longid memberId /* playerId or unionId */)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);
            MyDebug.Assert(serviceType == null);

            double? r = await GetDb().SortedSetScoreAsync(key, memberId);
            if (r == null)
            {
                return 0;
            }
            else
            {
                return TimeUtils.RemoveTimeFactor(r.Value);
            }
        }

        // public async Task<SortedSetEntry[]> GetAll(RankingListConfig config, IRankingListParamProvider provider)
        // {
        //     (RedisKey key, ServiceType? serviceType) = Key(config, provider);
        //     return await GetDb().SortedSetRangeByRankWithScoresAsync(key, order: Order.Descending);
        // }

        // public async Task<double> GetScoreByRank(RankingListConfig config, IRankingListParamProvider provider, int rank)
        // {
        //     (RedisKey key, ServiceType? serviceType) = Key(config, provider);
        //     SortedSetEntry[] sortedSetEntries = await GetDb().SortedSetRangeByRankWithScoresAsync(key, rank - 1, rank - 1, Order.Descending);
        //     if (sortedSetEntries.Length > 0)
        //     {
        //         return TimeUtils.RemoveTimeFactor(sortedSetEntries[0].Score);
        //     }
        //     return 0;
        // }

        public async Task<int> GetLength(stTwoServerId twoServerId, RankingListConfig config, IRankingListParamProvider provider, NormalService service)
        {
            (RedisKey key, ServiceType? serviceType) = Key(twoServerId, config, provider);

            long length = 0;
            if (serviceType == null)
            {
                length = await GetDb().SortedSetLengthAsync(key);
            }
            else
            {
                bool b = service.connectToOtherServiceDict.TryGetValue(serviceType.Value, out ConnectToOtherService connectToOtherService);
                if (!b)
                {
                    MyDebug.Assert(false);
                    return 0;
                }

                var r = await connectToOtherService.SendAsync(MsgType._RankLength, new MsgRankLength { key = key });
                if (r.err == ECode.Success)
                {
                    length = (int)r.CastRes<ResRankLength>().length;
                }
            }

            return (int)this.LimitLength(config, length);
        }

        long LimitLength(RankingListConfig config, long length)
        {
            if (config.limit > 0 && length > config.limit)
            {
                return config.limit;
            }
            return length;
        }

        int LimitRank(RankingListConfig config, int rank)
        {
            if (config.limit > 0 && rank > config.limit)
            {
                return RankUtils.INVALID_RANK;
            }
            return rank;
        }
    }
}