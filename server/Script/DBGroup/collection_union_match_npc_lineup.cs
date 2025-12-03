//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_match_npc_lineup : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "union_match_npc_lineup";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionMatchNpcLineup> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionMatchNpcLineup> collection = database.GetCollection<UnionMatchNpcLineup>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public IMongoCollection<UnionMatchNpcLineup_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionMatchNpcLineup_Db> collection = database.GetCollection<UnionMatchNpcLineup_Db>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionMatchNpcLineup.npcId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionMatchNpcLineup> Query_UnionMatchNpcLineup_by_npcId(string npcId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionMatchNpcLineup>.Filter.Eq(nameof(UnionMatchNpcLineup.npcId), npcId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionMatchNpcLineup info)
    {
        var collection = this.GetCollection_Db();
        var filter = Builders<UnionMatchNpcLineup_Db>.Filter.Eq(nameof(UnionMatchNpcLineup_Db.npcId), info.npcId);
        var info_Db = ProfileHelper_Db.Copy_Class<UnionMatchNpcLineup_Db, UnionMatchNpcLineup>(info);
        await collection.ReplaceOneAsync(filter, info_Db, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
