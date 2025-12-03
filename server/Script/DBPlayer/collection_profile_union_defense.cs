//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_union_defense : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "profile_union_defense";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileUnionDefense> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileUnionDefense> collection = database.GetCollection<ProfileUnionDefense>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileUnionDefense.season), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileUnionDefense> Query_ProfileUnionDefense_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionDefense>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileUnionDefense info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionDefense>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
