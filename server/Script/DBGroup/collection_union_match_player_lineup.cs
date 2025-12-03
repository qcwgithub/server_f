//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_match_player_lineup : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "union_match_player_lineup";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionMatchPlayerLineup> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionMatchPlayerLineup> collection = database.GetCollection<UnionMatchPlayerLineup>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public IMongoCollection<UnionMatchPlayerLineup_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionMatchPlayerLineup_Db> collection = database.GetCollection<UnionMatchPlayerLineup_Db>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> { nameof(UnionMatchPlayerLineup.playerId), nameof(UnionMatchPlayerLineup.lineupIndex) }, true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionMatchPlayerLineup> Query_UnionMatchPlayerLineup_by_playerId_lineupIndex(longid playerId,int lineupIndex)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<UnionMatchPlayerLineup>.Filter.Eq(nameof(UnionMatchPlayerLineup.playerId), playerId);
        var eq2 = Builders<UnionMatchPlayerLineup>.Filter.Eq(nameof(UnionMatchPlayerLineup.lineupIndex), lineupIndex);
        var filter = Builders<UnionMatchPlayerLineup>.Filter.And(eq1, eq2);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionMatchPlayerLineup info)
    {
        var collection = this.GetCollection_Db();
        var eq1 = Builders<UnionMatchPlayerLineup_Db>.Filter.Eq(nameof(UnionMatchPlayerLineup_Db.playerId), info.playerId);
        var eq2 = Builders<UnionMatchPlayerLineup_Db>.Filter.Eq(nameof(UnionMatchPlayerLineup_Db.lineupIndex), info.lineupIndex);
        var filter = Builders<UnionMatchPlayerLineup_Db>.Filter.And(eq1, eq2);
        var info_Db = ProfileHelper_Db.Copy_Class<UnionMatchPlayerLineup_Db, UnionMatchPlayerLineup>(info);
        await collection.ReplaceOneAsync(filter, info_Db, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
