//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;
using ExpeditionPartyTeamInfo = Data.TeamInfo;


//// AUTO CREATED ////
public  class collection_expedition_party_team_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "expedition_party_team_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ExpeditionPartyTeamInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ExpeditionPartyTeamInfo> collection = database.GetCollection<ExpeditionPartyTeamInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ExpeditionPartyTeamInfo.teamId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ExpeditionPartyTeamInfo> Query_ExpeditionPartyTeamInfo_by_teamId(long teamId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPartyTeamInfo>.Filter.Eq(nameof(ExpeditionPartyTeamInfo.teamId), teamId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ExpeditionPartyTeamInfo>> Iterate_listOf_ExpeditionPartyTeamInfo_by_teamId(long start_teamId,long end_teamId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_teamId < end_teamId);
        var gte = Builders<ExpeditionPartyTeamInfo>.Filter.Gte(nameof(ExpeditionPartyTeamInfo.teamId), start_teamId);
        var lt = Builders<ExpeditionPartyTeamInfo>.Filter.Lt(nameof(ExpeditionPartyTeamInfo.teamId), end_teamId);
        var filter = Builders<ExpeditionPartyTeamInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ExpeditionPartyTeamInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPartyTeamInfo>.Filter.Eq(nameof(ExpeditionPartyTeamInfo.teamId), info.teamId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
