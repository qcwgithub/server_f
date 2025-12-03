//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_group_server_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "group_server_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<GroupServerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<GroupServerInfo> collection = database.GetCollection<GroupServerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<GroupServerInfo> Query_GroupServerInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupServerInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(GroupServerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupServerInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
