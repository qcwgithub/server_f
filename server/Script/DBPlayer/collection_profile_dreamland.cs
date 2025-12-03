//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_dreamland : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "dreamland";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileDreamland> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileDreamland> collection = database.GetCollection<ProfileDreamland>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ProfileDreamland> Query_ProfileDreamland_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileDreamland>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileDreamland info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileDreamland>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
