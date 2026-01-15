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

        public async Task Add(long roomId, byte[] bytes)
        {
            long length = await this.GetDb().ListRightPushAsync(Key(roomId), bytes);
            if (length > 10000)
            {
                await this.GetDb().ListTrimAsync(Key(roomId), -10000, -1);
            }
        }

        // public async Task Add(ChatMessage message)
        // {
        //     MyDebug.Assert(message.roomId > 0);
        //     byte[] bytes = MessagePackSerializer.Serialize(message);
        //     long length = await this.GetDb().ListRightPushAsync(Key(message.roomId), bytes);
        //     if (length > 10000)
        //     {
        //         await this.GetDb().ListTrimAsync(Key(message.roomId), -10000, -1);
        //     }
        // }
    }
}