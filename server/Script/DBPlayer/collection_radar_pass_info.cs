//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_radar_pass_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "radar_pass_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<RadarPassInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<RadarPassInfo> collection = database.GetCollection<RadarPassInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<RadarPassInfo> Query_RadarPassInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<RadarPassInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(RadarPassInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<RadarPassInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
