//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public partial  class collection_champion_group_robot_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "champion_group_robot_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ChampionGroupRobotInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionGroupRobotInfo> collection = database.GetCollection<ChampionGroupRobotInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> { nameof(ChampionGroupRobotInfo.groupType), nameof(ChampionGroupRobotInfo.groupId) }, true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ChampionGroupRobotInfo> Query_ChampionGroupRobotInfo_by_groupType_groupId(int groupType,int groupId)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<ChampionGroupRobotInfo>.Filter.Eq(nameof(ChampionGroupRobotInfo.groupType), groupType);
        var eq2 = Builders<ChampionGroupRobotInfo>.Filter.Eq(nameof(ChampionGroupRobotInfo.groupId), groupId);
        var filter = Builders<ChampionGroupRobotInfo>.Filter.And(eq1, eq2);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ChampionGroupRobotInfo info)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<ChampionGroupRobotInfo>.Filter.Eq(nameof(ChampionGroupRobotInfo.groupType), info.groupType);
        var eq2 = Builders<ChampionGroupRobotInfo>.Filter.Eq(nameof(ChampionGroupRobotInfo.groupId), info.groupId);
        var filter = Builders<ChampionGroupRobotInfo>.Filter.And(eq1, eq2);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
