//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_original_mail : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "original_mail";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileOriginalMail> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileOriginalMail> collection = database.GetCollection<ProfileOriginalMail>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileOriginalMail.mailId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileOriginalMail.deleted), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileOriginalMail> Query_ProfileOriginalMail_by_mailId(longid mailId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileOriginalMail>.Filter.Eq(nameof(ProfileOriginalMail.mailId), mailId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<longid> Query_ProfileOriginalMail_maxOf_mailId()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileOriginalMail>.Filter.Gt(nameof(ProfileOriginalMail.mailId), 0);
        var projection = Builders<ProfileOriginalMail>.Projection.Include(nameof(ProfileOriginalMail.mailId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.mailId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (longid)result[0][nameof(ProfileOriginalMail.mailId)] : default(longid);
    }

    //// AUTO CREATED ////
    public async Task<List<ProfileOriginalMail>> Iterate_listOf_ProfileOriginalMail_by_mailId_nd(longid start_mailId,longid end_mailId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_mailId < end_mailId);
        var gte = Builders<ProfileOriginalMail>.Filter.Gte(nameof(ProfileOriginalMail.mailId), start_mailId);
        var lt = Builders<ProfileOriginalMail>.Filter.Lt(nameof(ProfileOriginalMail.mailId), end_mailId);
        var nd = Builders<ProfileOriginalMail>.Filter.Ne(nameof(ProfileOriginalMail.deleted), true);
        var filter = Builders<ProfileOriginalMail>.Filter.And(gte, lt, nd);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileOriginalMail info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileOriginalMail>.Filter.Eq(nameof(ProfileOriginalMail.mailId), info.mailId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
