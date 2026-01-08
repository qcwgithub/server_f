using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_room_info
{
    public async Task<List<RoomInfo>> Search(string keyword)
    {
        var collection = this.GetCollection();

        // Regex.Escape(keyword) 防止用户输入特殊正则字符崩掉查询
        var regex = new BsonRegularExpression(new Regex(Regex.Escape(keyword), RegexOptions.IgnoreCase));

        var f_title = Builders<RoomInfo>.Filter.Regex(nameof(RoomInfo.title), regex);
        var f_desc = Builders<RoomInfo>.Filter.Regex(nameof(RoomInfo.desc), regex);
        var filter = Builders<RoomInfo>.Filter.Or(f_title, f_desc);

        var find = collection.Find(filter)
            .Limit(20); // 分页

        var result = await find.ToListAsync();
        return result;
    }

    public IMongoCollection<RoomInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<RoomInfo_Db> collection = database.GetCollection<RoomInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(RoomInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<RoomInfo_Db, RoomInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, RoomInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<RoomInfo_Db>.Filter.Eq(nameof(RoomInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<RoomInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<RoomInfo_Db>.Update.Set(nameof(RoomInfo_Db.roomId), roomId_Db)
                : Builders<RoomInfo_Db>.Update.Unset(nameof(RoomInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<RoomInfo_Db>.Update.Set(nameof(RoomInfo_Db.createTimeS), createTimeS_Db)
                : Builders<RoomInfo_Db>.Update.Unset(nameof(RoomInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.title != null)
        {
            var title_Db = XInfoHelper_Db.Copy_string(infoNullable.title);
            var upd = title_Db != null
                ? Builders<RoomInfo_Db>.Update.Set(nameof(RoomInfo_Db.title), title_Db)
                : Builders<RoomInfo_Db>.Update.Unset(nameof(RoomInfo_Db.title));
            updList.Add(upd);
        }

        if (infoNullable.desc != null)
        {
            var desc_Db = XInfoHelper_Db.Copy_string(infoNullable.desc);
            var upd = desc_Db != null
                ? Builders<RoomInfo_Db>.Update.Set(nameof(RoomInfo_Db.desc), desc_Db)
                : Builders<RoomInfo_Db>.Update.Unset(nameof(RoomInfo_Db.desc));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<RoomInfo_Db> finalUpd = Builders<RoomInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}