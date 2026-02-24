//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_user_friend_chat_in_box : ServiceScript<DbService>
{
    public const string COLLECTION = "user_friend_chat_in_box";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_user_friend_chat_in_box(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<UserFriendChatInBoxItem> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserFriendChatInBoxItem> collection = database.GetCollection<UserFriendChatInBoxItem>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UserFriendChatInBoxItem.itemId), true, true, this.service.logger);
    }
}

//// AUTO CREATED ////
