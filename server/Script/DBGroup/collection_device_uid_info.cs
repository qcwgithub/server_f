//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public partial  class collection_device_uid_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "device_uid_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileDeviceUidInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileDeviceUidInfo> collection = database.GetCollection<ProfileDeviceUidInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileDeviceUidInfo.deviceUid), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileDeviceUidInfo> Query_ProfileDeviceUidInfo_by_deviceUid(string deviceUid)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileDeviceUidInfo>.Filter.Eq(nameof(ProfileDeviceUidInfo.deviceUid), deviceUid);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileDeviceUidInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileDeviceUidInfo>.Filter.Eq(nameof(ProfileDeviceUidInfo.deviceUid), info.deviceUid);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
