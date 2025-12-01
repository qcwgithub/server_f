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
    public sealed class ProfileApexGroupProxy : DataProxy<ProfileApexGroup, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid groupId, int _2 = 0)
        {
            return stDirtyElement.Create_ProfileApexGroup(groupId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid groupId, int _2 = 0)
        {
            return ApexKey.Group(groupId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ProfileApexGroup> OnlyForSave_GetFromRedis(longid groupId)
        {
            return this.GetFromRedis(groupId, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileApexGroup CreatePlaceholder(longid groupId, int _2 = 0)
        {
            var placeholder = new ProfileApexGroup();
            placeholder.groupId = groupId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid groupId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileApexGroup(groupId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileApexGroup)> LoadFromDB(IConnectToDBService connectToDBService, longid groupId, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileApexGroup_by_groupId();
            msgDb.groupId = groupId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileApexGroup_by_groupId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileApexGroup_by_groupId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid groupId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileApexGroup.ToTaskQueueHash(groupId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileApexGroup> Get(ConnectToDBGroupService connectToDBGroupService, longid groupId)
        {
            if (groupId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, groupId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.groupId == groupId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileApexGroup info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.groupId, 0, info);
        }
    }
}
