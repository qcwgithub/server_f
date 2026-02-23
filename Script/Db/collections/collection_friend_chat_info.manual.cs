using Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Script;
using System.Text.RegularExpressions;

public partial class collection_friend_chat_info
{
    public IMongoCollection<FriendChatInfo_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<FriendChatInfo_Db> collection = database.GetCollection<FriendChatInfo_Db>(COLLECTION);
        return collection;
    }

    public async Task Insert(FriendChatInfo info)
    {
        var info_Db = XInfoHelper_Db.Copy_Class<FriendChatInfo_Db, FriendChatInfo>(info);

        var collection_Db = this.GetCollection_Db();
        await collection_Db.InsertOneAsync(info_Db);
    }

    public async Task<ECode> Save(long roomId, FriendChatInfoInfoNullable infoNullable)
    {
        var collection_Db = this.GetCollection_Db();

        var filter = Builders<FriendChatInfo_Db>.Filter.Eq(nameof(FriendChatInfo_Db.roomId), roomId);
        var updList = new List<UpdateDefinition<FriendChatInfo_Db>>();

        #region autoSave

        if (infoNullable.roomId != null)
        {
            var roomId_Db = XInfoHelper_Db.Copy_long(infoNullable.roomId.Value);
            var upd = roomId_Db != null
                ? Builders<FriendChatInfo_Db>.Update.Set(nameof(FriendChatInfo_Db.roomId), roomId_Db)
                : Builders<FriendChatInfo_Db>.Update.Unset(nameof(FriendChatInfo_Db.roomId));
            updList.Add(upd);
        }

        if (infoNullable.createTimeS != null)
        {
            var createTimeS_Db = XInfoHelper_Db.Copy_long(infoNullable.createTimeS.Value);
            var upd = createTimeS_Db != null
                ? Builders<FriendChatInfo_Db>.Update.Set(nameof(FriendChatInfo_Db.createTimeS), createTimeS_Db)
                : Builders<FriendChatInfo_Db>.Update.Unset(nameof(FriendChatInfo_Db.createTimeS));
            updList.Add(upd);
        }

        if (infoNullable.messageSeq != null)
        {
            var messageSeq_Db = XInfoHelper_Db.Copy_long(infoNullable.messageSeq.Value);
            var upd = messageSeq_Db != null
                ? Builders<FriendChatInfo_Db>.Update.Set(nameof(FriendChatInfo_Db.messageSeq), messageSeq_Db)
                : Builders<FriendChatInfo_Db>.Update.Unset(nameof(FriendChatInfo_Db.messageSeq));
            updList.Add(upd);
        }

        if (infoNullable.users != null)
        {
            var users_Db = XInfoHelper_Db.Copy_ListClass<PrivateRoomUser_Db, PrivateRoomUser>(infoNullable.users);
            var upd = users_Db != null
                ? Builders<FriendChatInfo_Db>.Update.Set(nameof(FriendChatInfo_Db.users), users_Db)
                : Builders<FriendChatInfo_Db>.Update.Unset(nameof(FriendChatInfo_Db.users));
            updList.Add(upd);
        }


        #endregion autoSave

        UpdateDefinition<FriendChatInfo_Db> finalUpd = Builders<FriendChatInfo_Db>.Update.Combine(updList);
        var result = await collection_Db.UpdateOneAsync(filter, finalUpd, new UpdateOptions { IsUpsert = true });
        if (result.IsModifiedCountAvailable)
            return ECode.Success;
        else
            return ECode.DBErrorAffectedRowCount;
    }
}