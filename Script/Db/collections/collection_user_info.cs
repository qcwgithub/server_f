//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_user_info : ServiceScript<DbService>
{
    public const string COLLECTION = "user_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_user_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<UserInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserInfo> collection = database.GetCollection<UserInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UserInfo.userId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UserInfo.userName), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UserInfo> Query_UserInfo_by_userId(long userId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UserInfo>.Filter.Eq(nameof(UserInfo.userId), userId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<long> Query_UserInfo_maxOf_userId()
    {
        var collection = this.GetCollection();
        var filter = Builders<UserInfo>.Filter.Gt(nameof(UserInfo.userId), 0);
        var projection = Builders<UserInfo>.Projection.Include(nameof(UserInfo.userId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.userId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (long)result[0][nameof(UserInfo.userId)] : default(long);
    }
}

//// AUTO CREATED ////
