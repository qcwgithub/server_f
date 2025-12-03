//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_account_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "account_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<AccountInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<AccountInfo> collection = database.GetCollection<AccountInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> { nameof(AccountInfo.channel), nameof(AccountInfo.channelUserId) }, true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(AccountInfo.playerIds), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(AccountInfo.channelUserId_2), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<AccountInfo> Query_AccountInfo_by_channel_channelUserId(string channel,string channelUserId)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channel), channel);
        var eq2 = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channelUserId), channelUserId);
        var filter = Builders<AccountInfo>.Filter.And(eq1, eq2);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<AccountInfo> Query_AccountInfo_by_channelUserId(string channelUserId)
    {
        var collection = this.GetCollection();
        var filter = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channelUserId), channelUserId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<AccountInfo> Query_AccountInfo_by_channelUserId_2(string channelUserId_2)
    {
        var collection = this.GetCollection();
        var filter = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channelUserId_2), channelUserId_2);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<AccountInfo> Query_AccountInfo_byElementOf_playerIds(longid ele_playerIds)
    {
        var collection = this.GetCollection();
        var filter = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.playerIds), ele_playerIds);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<AccountInfo>> Query_listOf_AccountInfo_byElementOf_playerIds(longid ele_playerIds)
    {
        var collection = this.GetCollection();
        var filter = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.playerIds), ele_playerIds);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(AccountInfo info)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channel), info.channel);
        var eq2 = Builders<AccountInfo>.Filter.Eq(nameof(AccountInfo.channelUserId), info.channelUserId);
        var filter = Builders<AccountInfo>.Filter.And(eq1, eq2);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
