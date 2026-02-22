//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_friend_chat_info : ServiceScript<DbService>
{
    public const string COLLECTION = "friend_chat_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_friend_chat_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<FriendChatInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<FriendChatInfo> collection = database.GetCollection<FriendChatInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(FriendChatInfo.roomId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<FriendChatInfo> Query_FriendChatInfo_by_roomId(long roomId)
    {
        var collection = this.GetCollection();
        var filter = Builders<FriendChatInfo>.Filter.Eq(nameof(FriendChatInfo.roomId), roomId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<long> Query_FriendChatInfo_maxOf_roomId()
    {
        var collection = this.GetCollection();
        var filter = Builders<FriendChatInfo>.Filter.Gt(nameof(FriendChatInfo.roomId), 0);
        var projection = Builders<FriendChatInfo>.Projection.Include(nameof(FriendChatInfo.roomId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.roomId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (long)result[0][nameof(FriendChatInfo.roomId)] : default(long);
    }
}

//// AUTO CREATED ////
