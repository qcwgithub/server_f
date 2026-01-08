//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_room_info : ServiceScript<DbService>
{
    public const string COLLECTION = "room_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_room_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<RoomInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<RoomInfo> collection = database.GetCollection<RoomInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(RoomInfo.roomId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(RoomInfo.title), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(RoomInfo.desc), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<RoomInfo> Query_RoomInfo_by_roomId(long roomId)
    {
        var collection = this.GetCollection();
        var filter = Builders<RoomInfo>.Filter.Eq(nameof(RoomInfo.roomId), roomId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<long> Query_RoomInfo_maxOf_roomId()
    {
        var collection = this.GetCollection();
        var filter = Builders<RoomInfo>.Filter.Gt(nameof(RoomInfo.roomId), 0);
        var projection = Builders<RoomInfo>.Projection.Include(nameof(RoomInfo.roomId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.roomId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (long)result[0][nameof(RoomInfo.roomId)] : default(long);
    }
}

//// AUTO CREATED ////
