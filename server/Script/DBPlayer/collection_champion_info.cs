//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_champion_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "champion_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ChampionInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionInfo> collection = database.GetCollection<ChampionInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ChampionInfo> Query_ChampionInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ChampionInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
