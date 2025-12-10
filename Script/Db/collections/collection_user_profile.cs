using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;

public class collection_user_profile : ServiceScript<DbService>
{
    public const string COLLECTION = "user_profile";

    public collection_user_profile(Server server, DbService service) : base(server, service)
    {
    }


    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    public IMongoCollection<Profile> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<Profile> collection = database.GetCollection<Profile>(COLLECTION);
        return collection;
    }
    public IMongoCollection<Profile_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<Profile_Db> collection = database.GetCollection<Profile_Db>(COLLECTION);
        return collection;
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<T> collection = database.GetCollection<T>(COLLECTION);
        return collection;
    }

    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION,
            nameof(Profile.userId), true, true, this.service.logger);

        // RegularSearchByName 要用到
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION,
            nameof(Profile.userName), true, false, this.service.logger);
    }

    public async Task<Profile> Query(long userId)
    {
        var collection = this.GetCollection();

        var filter = Builders<Profile>.Filter.Eq(nameof(Profile.userId), userId);
        var find = await collection.FindAsync(filter);
        Profile profile = await find.FirstOrDefaultAsync();
        return profile;
    }

    public async Task<Dictionary<long, Profile>> Iterate_dictOf_Profile_by_userId(long start_userId, long end_userId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_userId < end_userId);
        var gte = Builders<Profile>.Filter.Gte(nameof(Profile.userId), start_userId);
        var lt = Builders<Profile>.Filter.Lt(nameof(Profile.userId), end_userId);
        var filter = Builders<Profile>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        var dict = new Dictionary<long, Profile>();
        foreach (var doc in result)
        {
            dict[doc.userId] = doc;
        }
        return dict;
    }

    public async Task Insert(Profile profile)
    {
        var profile_Db = ProfileHelper_Db.Copy_Class<Profile_Db, Profile>(profile);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(profile_Db);
    }

    public async Task<List<long>> RegularSearchByName(string searchText)
    {
        var collection_Db = this.GetCollection_Db();

        // "i" 表示忽略大小写
        var filter = Builders<Profile_Db>.Filter.Regex(nameof(Profile_Db.userName), new BsonRegularExpression(searchText, "i"));
        var projection = Builders<Profile_Db>.Projection.Include(nameof(Profile_Db.userId));
        var find = collection_Db
            .Find(filter)
            .Limit(100)
            .Project(projection);

        var result = await find.ToListAsync();
        List<long> userIds = result.Select(r => (long)r[nameof(Profile_Db.userId)]).ToList();
        return userIds;
    }

    public async Task<ECode> Save(long userId, ProfileNullable profileNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<Profile_Db>.Filter.Eq(nameof(Profile_Db.userId), userId);
        var updList = new List<UpdateDefinition<Profile_Db>>();
        #region autoSave

        if (profileNullable.userId != null)
        {
            var userId_Db = ProfileHelper_Db.Copy_long(profileNullable.userId.Value);
            var upd = userId_Db != null
                ? Builders<Profile_Db>.Update.Set(nameof(Profile_Db.userId), userId_Db)
                : Builders<Profile_Db>.Update.Unset(nameof(Profile_Db.userId));
            updList.Add(upd);
        }

        if (profileNullable.userName != null)
        {
            var userName_Db = ProfileHelper_Db.Copy_string(profileNullable.userName);
            var upd = userName_Db != null
                ? Builders<Profile_Db>.Update.Set(nameof(Profile_Db.userName), userName_Db)
                : Builders<Profile_Db>.Update.Unset(nameof(Profile_Db.userName));
            updList.Add(upd);
        }

        if (profileNullable.createTimeS != null)
        {
            var createTimeS_Db = ProfileHelper_Db.Copy_long(profileNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<Profile_Db>.Update.Set(nameof(Profile_Db.createTimeS), createTimeS_Db)
                : Builders<Profile_Db>.Update.Unset(nameof(Profile_Db.createTimeS));
            updList.Add(upd);
        }

        if (profileNullable.lastLoginTimeS != null)
        {
            var lastLoginTimeS_Db = ProfileHelper_Db.Copy_long(profileNullable.lastLoginTimeS.Value);
            var upd = lastLoginTimeS_Db != null
                ? Builders<Profile_Db>.Update.Set(nameof(Profile_Db.lastLoginTimeS), lastLoginTimeS_Db)
                : Builders<Profile_Db>.Update.Unset(nameof(Profile_Db.lastLoginTimeS));
            updList.Add(upd);
        }


        #endregion autoSave
        UpdateDefinition<Profile_Db> finalUpd = Builders<Profile_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}