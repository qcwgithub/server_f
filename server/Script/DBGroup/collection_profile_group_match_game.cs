//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_group_match_game : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_group_match_game";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileGroupMatchGame> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileGroupMatchGame> collection = database.GetCollection<ProfileGroupMatchGame>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ProfileGroupMatchGame> Query_ProfileGroupMatchGame_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupMatchGame>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileGroupMatchGame info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupMatchGame>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
