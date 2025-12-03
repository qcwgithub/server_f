//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_apex_robot_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "apex_robot_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ApexRobotInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ApexRobotInfo> collection = database.GetCollection<ApexRobotInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ApexRobotInfo> Query_ApexRobotInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexRobotInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ApexRobotInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexRobotInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
