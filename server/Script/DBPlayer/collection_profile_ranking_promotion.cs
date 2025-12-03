//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_ranking_promotion : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "ranking_promotion";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileRankingPromotion> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileRankingPromotion> collection = database.GetCollection<ProfileRankingPromotion>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ProfileRankingPromotion> Query_ProfileRankingPromotion_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileRankingPromotion>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileRankingPromotion info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileRankingPromotion>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
