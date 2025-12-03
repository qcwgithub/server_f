//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_group_union_defense : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_group_union_defense";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileGroupUnionDefense> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileGroupUnionDefense> collection = database.GetCollection<ProfileGroupUnionDefense>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ProfileGroupUnionDefense> Query_ProfileGroupUnionDefense_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupUnionDefense>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileGroupUnionDefense info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGroupUnionDefense>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
