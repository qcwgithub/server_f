using Data;
using MongoDB.Driver;
using Script;

public class collection_friend_chat_message : ServiceScript<DbService>
{
    public const string COLLECTION = "friend_chat_message";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    public collection_friend_chat_message(Server server, DbService service) : base(server, service)
    {
    }

    public IMongoCollection<ChatMessage> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChatMessage> collection = database.GetCollection<ChatMessage>(COLLECTION);
        return collection;
    }

    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION,
            [nameof(ChatMessage.roomId), nameof(ChatMessage.messageId)],
            ascending: true, isUnique: true,
            this.service.logger);
    }

    public async Task Save(ChatMessage[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            return;
        }

        var collection = this.GetCollection();

        try
        {
            await collection.InsertManyAsync(messages, new InsertManyOptions
            {
                IsOrdered = false // 非顺序插入，提高吞吐 & 避免一条失败影响全部
            });
        }
        catch (MongoBulkWriteException<ChatMessage> ex)
        {
            // 只处理“重复 key”这种情况（比如 messageId 唯一索引冲突）
            foreach (var error in ex.WriteErrors)
            {
                if (error.Category != ServerErrorCategory.DuplicateKey)
                {
                    this.service.logger.Error($"Mongo insert error: {error.Message}");
                }
            }

            // 可选：记录一下整体异常
            this.service.logger.Error($"Bulk insert partial failure, inserted: {messages.Length - ex.WriteErrors.Count}, failed: {ex.WriteErrors.Count}");
        }
        catch (Exception ex)
        {
            this.service.logger.Error("Mongo insert exception", ex);
        }
    }

    // roomIdToMessageIds
    //   Key = roomId
    //   Value = messageIds
    public async Task<List<ChatMessage>> Query(Dictionary<long, List<long>> roomIdToMessageIds)
    {
        if (roomIdToMessageIds == null || roomIdToMessageIds.Count == 0)
        {
            return [];
        }

        var collection = this.GetCollection();
        var builder = Builders<ChatMessage>.Filter;
        var filters = new List<FilterDefinition<ChatMessage>>();
        foreach (var kv in roomIdToMessageIds)
        {
            long roomId = kv.Key;
            List<long> messageIds = kv.Value;

            if (messageIds == null || messageIds.Count == 0)
            {
                continue;
            }

            var f = builder.And(builder.Eq(x => x.roomId, roomId), builder.In(x => x.messageId, messageIds));
            filters.Add(f);
        }
        if (filters.Count == 0)
        {
            return [];
        }

        var finalFilter = builder.Or(filters);

        var result = await collection.Find(finalFilter).ToListAsync();
        return result;
    }
}