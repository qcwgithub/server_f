//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public  class collection_user_brief_info : ServiceScript<DbService>
{
    public const string COLLECTION = "user_brief_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_user_brief_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<UserBriefInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserBriefInfo> collection = database.GetCollection<UserBriefInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UserBriefInfo.userId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UserBriefInfo> Query_UserBriefInfo_by_userId(long userId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UserBriefInfo>.Filter.Eq(nameof(UserBriefInfo.userId), userId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UserBriefInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UserBriefInfo>.Filter.Eq(nameof(UserBriefInfo.userId), info.userId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
