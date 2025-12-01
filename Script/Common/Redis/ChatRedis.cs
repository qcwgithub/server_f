using System.Linq;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System;

// 管理玩家聊天服务器数据

namespace Script
{
    public class ChatRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public async Task<long> AllocMsgId(stTwoServerId twoServerId, ChatChannel channel, string subChannel)
        {
            long r = await GetDb().StringIncrementAsync(ChatKey.MsgMaxId(twoServerId, channel, subChannel));
            return r;
        }

        public async Task AddChat(stTwoServerId twoServerId, ChatChannel channel, string subChannel, ChatMsgData data)
        {
            await Task.WhenAll(
                this.GetDb().SortedSetAddAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), JsonUtils.stringify(data), (double)data.id),
                this.GetDb().SortedSetAddAsync(ChatKey.MsgContentSingle(twoServerId, channel, subChannel, data.contentType.IsPlayer()), JsonUtils.stringify(data), (double)data.id));
        }

        public async Task<ChatMsgData> GetChatById(stTwoServerId twoServerId, ChatChannel channel, string subChannel, long id)
        {
            RedisValue[] values = await this.GetDb().SortedSetRangeByScoreAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), id, id);
            if (values.Length != 1)
            {
                return null;
            }
            return JsonUtils.parse<ChatMsgData>(values[0]);
        }

        public async Task RemoveChat(stTwoServerId twoServerId, ChatChannel channel, string subChannel, ChatMsgData data)
        {
            await Task.WhenAll(
                this.GetDb().SortedSetRemoveRangeByScoreAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), data.id, data.id),
                this.GetDb().SortedSetRemoveRangeByScoreAsync(ChatKey.MsgContentSingle(twoServerId, channel, subChannel, data.contentType.IsPlayer()), data.id, data.id));
        }

        public async Task GetChats(stTwoServerId twoServerId, ChatChannel channel, string subChannel, long chatMsgId, int getCount, /*OUT*/List<ChatMsgData> retList)
        {
            // 从最新的聊天信息获取（最大信息id开始遍历）
            if (chatMsgId == -1)
            {
                RedisValue[] values = await this.GetDb().SortedSetRangeByRankAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), 0, getCount - 1, Order.Descending);
                foreach (var v in values)
                {
                    if (!v.IsNullOrEmpty)
                    {
                        retList.Add(JsonUtils.parse<ChatMsgData>(v));
                    }
                }
            }
            else
            {
                // 小于（早于）chatMsgId排序的聊天记录
                long? rank = await this.GetDb().SortedSetRankAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), chatMsgId, order: Order.Descending);
                if (rank == null)
                {
                    return;
                }
                else
                {
                    long si = rank.Value;
                    RedisValue[] values = await this.GetDb().SortedSetRangeByRankAsync(ChatKey.MsgContentWhole(twoServerId, channel, subChannel), si, si + getCount - 1, Order.Descending);
                    foreach (var v in values)
                    {
                        if (!v.IsNullOrEmpty)
                        {
                            retList.Add(JsonUtils.parse<ChatMsgData>(v));
                        }
                    }
                }
            }
        }

        public async void TruncateChats(stTwoServerId twoServerId, ChatChannel channel, string subChannel, bool isPlayer, int max)
        {
            if (max <= 0)
            {
                return;
            }

            long length = await this.GetDb().SortedSetLengthAsync(ChatKey.MsgContentSingle(twoServerId, channel, subChannel, isPlayer));
            if (length <= max)
            {
                return;
            }

            var tasks = new List<Task>();

            RedisValue[] values = await this.GetDb().SortedSetRangeByRankAsync(ChatKey.MsgContentSingle(twoServerId, channel, subChannel, isPlayer), 0, length - max - 1);
            foreach (RedisValue value in values)
            {
                ChatMsgData data = JsonUtils.parse<ChatMsgData>(value);
                tasks.Add(this.RemoveChat(twoServerId, channel, subChannel, data));
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }
    }
}
