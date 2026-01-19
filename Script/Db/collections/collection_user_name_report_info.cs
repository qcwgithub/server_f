//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public  class collection_user_name_report_info : ServiceScript<DbService>
{
    public const string COLLECTION = "room_user_name_report_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_user_name_report_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<UserNameReportInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UserNameReportInfo> collection = database.GetCollection<UserNameReportInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UserNameReportInfo info)
    {
        var collection = this.GetCollection();
        await collection.InsertOneAsync(info);
        return ECode.Success;
    }
}

//// AUTO CREATED ////
