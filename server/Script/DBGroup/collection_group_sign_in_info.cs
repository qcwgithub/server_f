//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_group_sign_in_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "group_sign_in_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<GroupSignInInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<GroupSignInInfo> collection = database.GetCollection<GroupSignInInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<GroupSignInInfo> Query_GroupSignInInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupSignInInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(GroupSignInInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupSignInInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
