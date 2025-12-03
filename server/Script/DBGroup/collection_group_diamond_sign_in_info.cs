//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_group_diamond_sign_in_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "group_diamond_sign_in_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<GroupDiamondSignInInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<GroupDiamondSignInInfo> collection = database.GetCollection<GroupDiamondSignInInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<GroupDiamondSignInInfo> Query_GroupDiamondSignInInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupDiamondSignInInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(GroupDiamondSignInInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<GroupDiamondSignInInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
