//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_friend_chat_message : ServiceScript<DbService>
{
    public const string COLLECTION = "friend_chat_message";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_friend_chat_message(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<ChatMessage> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChatMessage> collection = database.GetCollection<ChatMessage>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> { nameof(ChatMessage.roomId), nameof(ChatMessage.messageId) }, true, true, this.service.logger);
    }
}

//// AUTO CREATED ////
