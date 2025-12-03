//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_tournament_rank : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_tournament_rank";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileTournamentRank> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileTournamentRank> collection = database.GetCollection<ProfileTournamentRank>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileTournamentRank.season), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileTournamentRank> Query_ProfileTournamentRank_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentRank>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileTournamentRank info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentRank>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
