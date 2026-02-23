using Data;
using MessagePack;
using StackExchange.Redis;

namespace Script
{
    public class FriendChatMessagesRedis : ServerScript
    {
        public FriendChatMessagesRedis(Server server) : base(server)
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

        string KeyDirty(long roomId)
        {
            return RoomKey.MessagesDirty(roomId);
        }

        public async Task Add(ChatMessage message)
        {
            MyDebug.Assert(message.roomId > 0);
            string key = Key(message.roomId);

            byte[] bytes = MessagePackSerializer.Serialize(message);
            await Task.WhenAll(
                this.GetDb().ListRightPushAsync(key, bytes),
                this.GetDb().StringIncrementAsync(KeyDirty(message.roomId)),
                this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.FriendChatMessagesEncode(message.roomId))
            );
        }

        public async Task<ChatMessage[]> GetAll(long roomId)
        {
            RedisValue[] redisValues = await this.GetDb().ListRangeAsync(Key(roomId), 0, -1);
            return redisValues.Select(x => MessagePackSerializer.Deserialize<ChatMessage>(x)).ToArray();
        }
    }
}