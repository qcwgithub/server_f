//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_union_competition : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_union_competition";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileUnionCompetition> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileUnionCompetition> collection = database.GetCollection<ProfileUnionCompetition>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileUnionCompetition.season), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileUnionCompetition> Query_ProfileUnionCompetition_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionCompetition>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileUnionCompetition info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionCompetition>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
