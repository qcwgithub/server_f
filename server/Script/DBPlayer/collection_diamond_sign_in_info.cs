//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_diamond_sign_in_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "diamond_sign_in_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<DiamondSignInInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<DiamondSignInInfo> collection = database.GetCollection<DiamondSignInInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(DiamondSignInInfo.season), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<DiamondSignInInfo> Query_DiamondSignInInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<DiamondSignInInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(DiamondSignInInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<DiamondSignInInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
