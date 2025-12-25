using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;

public partial class collection_room_info
{
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


        #endregion autoSave

        UpdateDefinition<RoomInfo_Db> finalUpd = Builders<RoomInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}