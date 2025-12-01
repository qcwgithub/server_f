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
    public sealed class GroupRadarPassInfoProxy : DataProxy<GroupRadarPassInfo, int, int>
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
            return stDirtyElement.Create_GroupRadarPassInfo();
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(int _1 = 0, int _2 = 0)
        {
            return GroupRadarPassKey.Info();
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return false;
        }

        //// AUTO CREATED ////
        public Task<GroupRadarPassInfo> OnlyForSave_GetFromRedis()
        {
            return this.GetFromRedis(0, 0);
        }

        //// AUTO CREATED ////
        protected override GroupRadarPassInfo CreatePlaceholder(int _1 = 0, int _2 = 0)
        {
            var placeholder = new GroupRadarPassInfo();
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(int _1 = 0, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.GroupRadarPassInfo();
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, GroupRadarPassInfo)> LoadFromDB(IConnectToDBService connectToDBService, int _1 = 0, int _2 = 0)
        {
            var msgDb = new MsgQuery_GroupRadarPassInfo_all();
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_GroupRadarPassInfo_all, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_GroupRadarPassInfo_all>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(int _1 = 0, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(GroupRadarPassInfo.ToTaskQueueHash());
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<GroupRadarPassInfo> Get(ConnectToDBGroupService connectToDBGroupService)
        {
            var info = await base.InternalGet(connectToDBGroupService, 0, 0);
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(GroupRadarPassInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(0, 0, info);
        }
    }
}
