using Data;
using MessagePack;
using StackExchange.Redis;

namespace Script
{
    public class PrivateRoomMessagesRedis : ServerScript
    {
        public PrivateRoomMessagesRedis(Server server) : base(server)
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
            await this.GetDb().SortedSetAddAsync(key, bytes, message.seq);
        }

        public async Task Trim(long roomId, int keepCount)
        {
            string key = Key(roomId);

            await this.GetDb().SortedSetRemoveRangeByRankAsync(key,
                start: 0,
                stop: -keepCount - 1);
        }

        // 返回值是从旧到新
        public async Task<List<ChatMessage>> GetRecents(long roomId, int count)
        {
            string key = Key(roomId);
            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: double.NegativeInfinity,
                stop: double.PositiveInfinity,
                order: Order.Descending,
                take: count);

            var list = new List<ChatMessage>();
            for (int i = redisValues.Length - 1; i >= 0; i--)
            {
                list.Add(MessagePackSerializer.Deserialize<ChatMessage>(redisValues[i]));
            }
            return list;
        }

        // 返回值是从旧到新
        public async Task<List<ChatMessage>> GetHistory(long roomId, long lastSeq, int count)
        {
            string key = Key(roomId);
            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: double.NegativeInfinity,
                stop: lastSeq,
                exclude: Exclude.Stop,
                order: Order.Descending,
                take: count);

            var list = new List<ChatMessage>();
            for (int i = redisValues.Length - 1; i >= 0; i--)
            {
                list.Add(MessagePackSerializer.Deserialize<ChatMessage>(redisValues[i]));
            }
            return list;
        }

        public async Task<ChatMessage?> QueryOne(long roomId, long seq)
        {
            string key = Key(roomId);

            RedisValue[] redisValues = await this.GetDb().SortedSetRangeByScoreAsync(key,
                start: seq,
                stop: seq,
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