//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_champion_robot_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "champion_robot_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ChampionRobotInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionRobotInfo> collection = database.GetCollection<ChampionRobotInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ChampionRobotInfo> Query_ChampionRobotInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionRobotInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ChampionRobotInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionRobotInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
