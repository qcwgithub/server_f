//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_group_roulette : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "group_roulette";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileGroupRoulette> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileGroupRoulette> collection = database.GetCollection<ProfileGroupRoulette>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ProfileGroupRoulette> Query_ProfileGroupRoulette_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupRoulette>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileGroupRoulette info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupRoulette>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
