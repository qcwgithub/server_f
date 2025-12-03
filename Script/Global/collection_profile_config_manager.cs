using Data;
using MongoDB.Driver;
using Script;

public  class collection_profile_global : ServiceScript<GlobalService>
{
    public const string COLLECTION = "profile_global";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;
    
    public IMongoCollection<ProfileGlobal> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileGlobal> collection = database.GetCollection<ProfileGlobal>(COLLECTION);
        return collection;
    }
    
    public async Task<ProfileGlobal> Query_ProfileGlobal_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGlobal>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }
    
    public async Task<ECode> Save(ProfileGlobal info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileGlobal>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}