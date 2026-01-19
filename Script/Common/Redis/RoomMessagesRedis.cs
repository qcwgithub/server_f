using Data;
using MessagePack;
using StackExchange.Redis;

namespace Script
{
    public class RoomMessagesRedis : ServerScript
    {
        public RoomMessagesRedis(Server server) : base(server)
        {

        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        string Key(long roomId)
        {
            return RoomKey.Messages(roomId);
        }

        public async Task Add(ChatMessage message)
        {
            MyDebug.Assert(message.roomId > 0);
            string key = Key(message.roomId);

            byte[] bytes = MessagePackSerializer.Serialize(message);
            await this.GetDb().SortedSetAddAsync(key, bytes, message.messageId);
        }

        public async Task Trim(long roomId, int keepCount)
        {
            string key = Key(roomId);

            await this.GetDb().SortedSetRemoveRangeByRankAsync(key,
                start: 0,
                stop: -keepCount - 1);
        }

        public async Task<List<ChatMessage>> GetRecents(long roomId, int count)
        {
            string key = Key(roomId);
            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: double.NegativeInfinity,
                stop: double.PositiveInfinity,
                order: Order.Descending,
                take: count);

            // 返回值是从新到旧
            return redisValues.Select(v => MessagePackSerializer.Deserialize<ChatMessage>(v)).ToList();
        }

        public async Task<List<ChatMessage>> GetHistory(long roomId, long lastMessageId, int count)
        {
            string key = Key(roomId);
            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: double.NegativeInfinity,
                stop: lastMessageId,
                exclude: Exclude.Stop,
                order: Order.Descending,
                take: count);

            // 返回值是从新到旧
            return redisValues.Select(v => MessagePackSerializer.Deserialize<ChatMessage>(v)).ToList();
        }

        public async Task<ChatMessage?> QueryOne(long roomId, long messageId)
        {
            string key = Key(roomId);

            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: messageId,
                stop: messageId,
                exclude: Exclude.None,
                order: Order.Descending,
                take: 1);

            if (redisValues.Length == 0)
            {
                return null;
            }

            return MessagePackSerializer.Deserialize<ChatMessage>(redisValues[0]);
        }
    }
}