using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;
using longid = System.Int64;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class ProfileUnionBrawlProxy : DataProxy<ProfileUnionBrawl, int, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(int _1 = 0, int _2 = 0)
        {
            return stDirtyElement.Create_ProfileUnionBrawl();
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(int _1 = 0, int _2 = 0)
        {
            return UnionBrawlKey.Profile();
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return false;
        }

        //// AUTO CREATED ////
        public Task<ProfileUnionBrawl> OnlyForSave_GetFromRedis()
        {
            return this.GetFromRedis(0, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileUnionBrawl CreatePlaceholder(int _1 = 0, int _2 = 0)
        {
            var placeholder = new ProfileUnionBrawl();
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(int _1 = 0, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileUnionBrawl();
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileUnionBrawl)> LoadFromDB(IConnectToDBService connectToDBService, int _1 = 0, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileUnionBrawl_all();
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileUnionBrawl_all, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileUnionBrawl_all>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(int _1 = 0, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileUnionBrawl.ToTaskQueueHash());
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileUnionBrawl> Get(ConnectToDBGroupService connectToDBGroupService)
        {
            var info = await base.InternalGet(connectToDBGroupService, 0, 0);
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileUnionBrawl info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(0, 0, info);
        }
    }
}
