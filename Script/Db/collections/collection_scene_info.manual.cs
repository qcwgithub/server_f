using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_scene_info
{
    public async Task<List<SceneInfo>> Search(string keyword)
    {
        var collection = this.GetCollection();

        // Regex.Escape(keyword) 防止用户输入特殊正则字符崩掉查询
        var regex = new BsonRegularExpression(new Regex(Regex.Escape(keyword), RegexOptions.IgnoreCase));

        var f_title = Builders<SceneInfo>.Filter.Regex(nameof(SceneInfo.title), regex);
        var f_desc = Builders<SceneInfo>.Filter.Regex(nameof(SceneInfo.desc), regex);
        var filter = Builders<SceneInfo>.Filter.Or(f_title, f_desc);

        var find = collection.Find(filter)
            .Limit(20); // 分页

        var result = await find.ToListAsync();
        return result;
    }

    public IMongoCollection<SceneInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<SceneInfo_Db> collection = database.GetCollection<SceneInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(SceneInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<SceneInfo_Db, SceneInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, SceneInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<SceneInfo_Db>.Filter.Eq(nameof(SceneInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<SceneInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<SceneInfo_Db>.Update.Set(nameof(SceneInfo_Db.roomId), roomId_Db)
                : Builders<SceneInfo_Db>.Update.Unset(nameof(SceneInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<SceneInfo_Db>.Update.Set(nameof(SceneInfo_Db.createTimeS), createTimeS_Db)
                : Builders<SceneInfo_Db>.Update.Unset(nameof(SceneInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.title != null)
        {
            var title_Db = XInfoHelper_Db.Copy_string(infoNullable.title);
            var upd = title_Db != null
                ? Builders<SceneInfo_Db>.Update.Set(nameof(SceneInfo_Db.title), title_Db)
                : Builders<SceneInfo_Db>.Update.Unset(nameof(SceneInfo_Db.title));
            updList.Add(upd);
        }

        if (infoNullable.desc != null)
        {
            var desc_Db = XInfoHelper_Db.Copy_string(infoNullable.desc);
            var upd = desc_Db != null
                ? Builders<SceneInfo_Db>.Update.Set(nameof(SceneInfo_Db.desc), desc_Db)
                : Builders<SceneInfo_Db>.Update.Unset(nameof(SceneInfo_Db.desc));
            updList.Add(upd);
        }

        if (infoNullable.seq != null)
        {
            var seq_Db = XInfoHelper_Db.Copy_long(infoNullable.seq.Value);
            var upd = seq_Db != null
                ? Builders<SceneInfo_Db>.Update.Set(nameof(SceneInfo_Db.seq), seq_Db)
                : Builders<SceneInfo_Db>.Update.Unset(nameof(SceneInfo_Db.seq));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<SceneInfo_Db> finalUpd = Builders<SceneInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}