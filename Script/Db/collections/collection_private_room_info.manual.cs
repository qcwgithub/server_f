using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_private_room_info
{
    public IMongoCollection<PrivateRoomInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PrivateRoomInfo_Db> collection = database.GetCollection<PrivateRoomInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(PrivateRoomInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<PrivateRoomInfo_Db, PrivateRoomInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, PrivateRoomInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<PrivateRoomInfo_Db>.Filter.Eq(nameof(PrivateRoomInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<PrivateRoomInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<PrivateRoomInfo_Db>.Update.Set(nameof(PrivateRoomInfo_Db.roomId), roomId_Db)
                : Builders<PrivateRoomInfo_Db>.Update.Unset(nameof(PrivateRoomInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<PrivateRoomInfo_Db>.Update.Set(nameof(PrivateRoomInfo_Db.createTimeS), createTimeS_Db)
                : Builders<PrivateRoomInfo_Db>.Update.Unset(nameof(PrivateRoomInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.messageId != null)
        {
            var messageId_Db = XInfoHelper_Db.Copy_long(infoNullable.messageId.Value);
            var upd = messageId_Db != null
                ? Builders<PrivateRoomInfo_Db>.Update.Set(nameof(PrivateRoomInfo_Db.messageId), messageId_Db)
                : Builders<PrivateRoomInfo_Db>.Update.Unset(nameof(PrivateRoomInfo_Db.messageId));
            updList.Add(upd);
        }

        if (infoNullable.participants != null)
        {
            var participants_Db = XInfoHelper_Db.Copy_ListClass<RoomParticipant_Db, RoomParticipant>(infoNullable.participants);
            var upd = participants_Db != null
                ? Builders<PrivateRoomInfo_Db>.Update.Set(nameof(PrivateRoomInfo_Db.participants), participants_Db)
                : Builders<PrivateRoomInfo_Db>.Update.Unset(nameof(PrivateRoomInfo_Db.participants));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<PrivateRoomInfo_Db> finalUpd = Builders<PrivateRoomInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}