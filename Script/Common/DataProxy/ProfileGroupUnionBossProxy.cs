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
    public sealed class ProfileGroupUnionBossProxy : DataProxy<ProfileGroupUnionBoss, int, int>
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
            return stDirtyElement.Create_ProfileGroupUnionBoss();
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(int _1 = 0, int _2 = 0)
        {
            return GroupUnionBossKey.Info();
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return false;
        }

        //// AUTO CREATED ////
        public Task<ProfileGroupUnionBoss> OnlyForSave_GetFromRedis()
        {
            return this.GetFromRedis(0, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileGroupUnionBoss CreatePlaceholder(int _1 = 0, int _2 = 0)
        {
            var placeholder = new ProfileGroupUnionBoss();
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(int _1 = 0, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileGroupUnionBoss();
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileGroupUnionBoss)> LoadFromDB(IConnectToDBService connectToDBService, int _1 = 0, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileGroupUnionBoss_all();
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileGroupUnionBoss_all, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileGroupUnionBoss_all>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(int _1 = 0, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileGroupUnionBoss.ToTaskQueueHash());
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileGroupUnionBoss> Get(ConnectToDBGroupService connectToDBGroupService)
        {
            var info = await base.InternalGet(connectToDBGroupService, 0, 0);
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileGroupUnionBoss info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(0, 0, info);
        }
    }
}
