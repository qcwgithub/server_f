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

    public async Task Insert(UserInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<UserInfo_Db, UserInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
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