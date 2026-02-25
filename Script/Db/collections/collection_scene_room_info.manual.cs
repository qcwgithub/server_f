using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_scene_room_info
{
    public async Task<List<SceneRoomInfo>> Search(string keyword)
    {
        var collection = this.GetCollection();

        // Regex.Escape(keyword) 防止用户输入特殊正则字符崩掉查询
        var regex = new BsonRegularExpression(new Regex(Regex.Escape(keyword), RegexOptions.IgnoreCase));

        var f_title = Builders<SceneRoomInfo>.Filter.Regex(nameof(SceneRoomInfo.title), regex);
        var f_desc = Builders<SceneRoomInfo>.Filter.Regex(nameof(SceneRoomInfo.desc), regex);
        var filter = Builders<SceneRoomInfo>.Filter.Or(f_title, f_desc);

        var find = collection.Find(filter)
            .Limit(20); // 分页

        var result = await find.ToListAsync();
        return result;
    }

    public IMongoCollection<SceneRoomInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<SceneRoomInfo_Db> collection = database.GetCollection<SceneRoomInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(SceneRoomInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<SceneRoomInfo_Db, SceneRoomInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, SceneRoomInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<SceneRoomInfo_Db>.Filter.Eq(nameof(SceneRoomInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<SceneRoomInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<SceneRoomInfo_Db>.Update.Set(nameof(SceneRoomInfo_Db.roomId), roomId_Db)
                : Builders<SceneRoomInfo_Db>.Update.Unset(nameof(SceneRoomInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<SceneRoomInfo_Db>.Update.Set(nameof(SceneRoomInfo_Db.createTimeS), createTimeS_Db)
                : Builders<SceneRoomInfo_Db>.Update.Unset(nameof(SceneRoomInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.title != null)
        {
            var title_Db = XInfoHelper_Db.Copy_string(infoNullable.title);
            var upd = title_Db != null
                ? Builders<SceneRoomInfo_Db>.Update.Set(nameof(SceneRoomInfo_Db.title), title_Db)
                : Builders<SceneRoomInfo_Db>.Update.Unset(nameof(SceneRoomInfo_Db.title));
            updList.Add(upd);
        }

        if (infoNullable.desc != null)
        {
            var desc_Db = XInfoHelper_Db.Copy_string(infoNullable.desc);
            var upd = desc_Db != null
                ? Builders<SceneRoomInfo_Db>.Update.Set(nameof(SceneRoomInfo_Db.desc), desc_Db)
                : Builders<SceneRoomInfo_Db>.Update.Unset(nameof(SceneRoomInfo_Db.desc));
            updList.Add(upd);
        }

        if (infoNullable.messageSeq != null)
        {
            var messageSeq_Db = XInfoHelper_Db.Copy_long(infoNullable.messageSeq.Value);
            var upd = messageSeq_Db != null
                ? Builders<SceneRoomInfo_Db>.Update.Set(nameof(SceneRoomInfo_Db.messageSeq), messageSeq_Db)
                : Builders<SceneRoomInfo_Db>.Update.Unset(nameof(SceneRoomInfo_Db.messageSeq));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<SceneRoomInfo_Db> finalUpd = Builders<SceneRoomInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}