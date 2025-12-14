using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;

public partial class collection_user_info
{
    public IMongoCollection<UserInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserInfo_Db> collection = database.GetCollection<UserInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task<UserInfo> Query(long userId)
    {
        var collection = this.GetCollection();

        var filter = Builders<UserInfo>.Filter.Eq(nameof(UserInfo.userId), userId);
        var find = await collection.FindAsync(filter);
        UserInfo info = await find.FirstOrDefaultAsync();
        return info;
    }

    public async Task<Dictionary<long, UserInfo>> Iterate_dictOf_Info_by_userId(long start_userId, long end_userId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_userId < end_userId);
        var gte = Builders<UserInfo>.Filter.Gte(nameof(UserInfo.userId), start_userId);
        var lt = Builders<UserInfo>.Filter.Lt(nameof(UserInfo.userId), end_userId);
        var filter = Builders<UserInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        var dict = new Dictionary<long, UserInfo>();
        foreach (var doc in result)
        {
            dict[doc.userId] = doc;
        }
        return dict;
    }

    public async Task Insert(UserInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<UserInfo_Db, UserInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<List<long>> RegularSearchByName(string searchText)
    {
        var collection_Db = this.GetCollection_Db();

        // "i" 表示忽略大小写
        var filter = Builders<UserInfo_Db>.Filter.Regex(nameof(UserInfo_Db.userName), new BsonRegularExpression(searchText, "i"));
        var projection = Builders<UserInfo_Db>.Projection.Include(nameof(UserInfo_Db.userId));
        var find = collection_Db
            .Find(filter)
            .Limit(100)
            .Project(projection);

        var result = await find.ToListAsync();
        List<long> userIds = result.Select(r => (long)r[nameof(UserInfo_Db.userId)]).ToList();
        return userIds;
    }

    public async Task<ECode> Save(long userId, UserInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<UserInfo_Db>.Filter.Eq(nameof(UserInfo_Db.userId), userId);
        var updList = new List<UpdateDefinition<UserInfo_Db>>();

        #region autoSave

        if (infoNullable.userId != null)
        {
            var userId_Db = XInfoHelper_Db.Copy_long(infoNullable.userId.Value);
            var upd = userId_Db != null
                ? Builders<UserInfo_Db>.Update.Set(nameof(UserInfo_Db.userId), userId_Db)
                : Builders<UserInfo_Db>.Update.Unset(nameof(UserInfo_Db.userId));
            updList.Add(upd);
        }

        if (infoNullable.userName != null)
        {
            var userName_Db = XInfoHelper_Db.Copy_string(infoNullable.userName);
            var upd = userName_Db != null
                ? Builders<UserInfo_Db>.Update.Set(nameof(UserInfo_Db.userName), userName_Db)
                : Builders<UserInfo_Db>.Update.Unset(nameof(UserInfo_Db.userName));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<UserInfo_Db>.Update.Set(nameof(UserInfo_Db.createTimeS), createTimeS_Db)
                : Builders<UserInfo_Db>.Update.Unset(nameof(UserInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.lastLoginTimeS != null)
        {
            var lastLoginTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.lastLoginTimeS.Value);
            var upd = lastLoginTimeS_Db != null
                ? Builders<UserInfo_Db>.Update.Set(nameof(UserInfo_Db.lastLoginTimeS), lastLoginTimeS_Db)
                : Builders<UserInfo_Db>.Update.Unset(nameof(UserInfo_Db.lastLoginTimeS));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<UserInfo_Db> finalUpd = Builders<UserInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}