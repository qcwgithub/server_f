//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_private_room_info : ServiceScript<DbService>
{
    public const string COLLECTION = "private_room_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_private_room_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<PrivateRoomInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PrivateRoomInfo> collection = database.GetCollection<PrivateRoomInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(PrivateRoomInfo.roomId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<PrivateRoomInfo> Query_PrivateRoomInfo_by_roomId(long roomId)
    {
        var collection = this.GetCollection();
        var filter = Builders<PrivateRoomInfo>.Filter.Eq(nameof(PrivateRoomInfo.roomId), roomId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<long> Query_PrivateRoomInfo_maxOf_roomId()
    {
        var collection = this.GetCollection();
        var filter = Builders<PrivateRoomInfo>.Filter.Gt(nameof(PrivateRoomInfo.roomId), 0);
        var projection = Builders<PrivateRoomInfo>.Projection.Include(nameof(PrivateRoomInfo.roomId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.roomId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (long)result[0][nameof(PrivateRoomInfo.roomId)] : default(long);
    }
}

//// AUTO CREATED ////
