//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_user_friend_chat_state : ServiceScript<DbService>
{
    public const string COLLECTION = "user_friend_chat_state";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_user_friend_chat_state(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<UserFriendChatState> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserFriendChatState> collection = database.GetCollection<UserFriendChatState>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UserFriendChatState.userId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UserFriendChatState> Query_UserFriendChatState_by_userId(long userId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UserFriendChatState>.Filter.Eq(nameof(UserFriendChatState.userId), userId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UserFriendChatState info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UserFriendChatState>.Filter.Eq(nameof(UserFriendChatState.userId), info.userId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
