using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_friend_chat_room_info
{
    public IMongoCollection<FriendChatRoomInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<FriendChatRoomInfo_Db> collection = database.GetCollection<FriendChatRoomInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(FriendChatRoomInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<FriendChatRoomInfo_Db, FriendChatRoomInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, FriendChatRoomInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<FriendChatRoomInfo_Db>.Filter.Eq(nameof(FriendChatRoomInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<FriendChatRoomInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<FriendChatRoomInfo_Db>.Update.Set(nameof(FriendChatRoomInfo_Db.roomId), roomId_Db)
                : Builders<FriendChatRoomInfo_Db>.Update.Unset(nameof(FriendChatRoomInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<FriendChatRoomInfo_Db>.Update.Set(nameof(FriendChatRoomInfo_Db.createTimeS), createTimeS_Db)
                : Builders<FriendChatRoomInfo_Db>.Update.Unset(nameof(FriendChatRoomInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.messageSeq != null)
        {
            var messageSeq_Db = XInfoHelper_Db.Copy_long(infoNullable.messageSeq.Value);
            var upd = messageSeq_Db != null
                ? Builders<FriendChatRoomInfo_Db>.Update.Set(nameof(FriendChatRoomInfo_Db.messageSeq), messageSeq_Db)
                : Builders<FriendChatRoomInfo_Db>.Update.Unset(nameof(FriendChatRoomInfo_Db.messageSeq));
            updList.Add(upd);
        }

        if (infoNullable.users != null)
        {
            var users_Db = XInfoHelper_Db.Copy_ListClass<FriendChatRoomUser_Db, FriendChatRoomUser>(infoNullable.users);
            var upd = users_Db != null
                ? Builders<FriendChatRoomInfo_Db>.Update.Set(nameof(FriendChatRoomInfo_Db.users), users_Db)
                : Builders<FriendChatRoomInfo_Db>.Update.Unset(nameof(FriendChatRoomInfo_Db.users));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<FriendChatRoomInfo_Db> finalUpd = Builders<FriendChatRoomInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}