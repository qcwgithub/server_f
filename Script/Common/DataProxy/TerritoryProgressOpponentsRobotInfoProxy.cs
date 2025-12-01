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
    public sealed class TerritoryProgressOpponentsRobotInfoProxy : DataProxy<TerritoryProgressOpponentsRobotInfo, int, int>
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
            return stDirtyElement.Create_TerritoryProgressOpponentsRobotInfo();
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(int _1 = 0, int _2 = 0)
        {
            return TerritoryProgressOpponentsKey.RobotInfo();
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return false;
        }

        //// AUTO CREATED ////
        public Task<TerritoryProgressOpponentsRobotInfo> OnlyForSave_GetFromRedis()
        {
            return this.GetFromRedis(0, 0);
        }

        //// AUTO CREATED ////
        protected override TerritoryProgressOpponentsRobotInfo CreatePlaceholder(int _1 = 0, int _2 = 0)
        {
            var placeholder = new TerritoryProgressOpponentsRobotInfo();
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(int _1 = 0, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.TerritoryProgressOpponentsRobotInfo();
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, TerritoryProgressOpponentsRobotInfo)> LoadFromDB(IConnectToDBService connectToDBService, int _1 = 0, int _2 = 0)
        {
            var msgDb = new MsgQuery_TerritoryProgressOpponentsRobotInfo_all();
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_TerritoryProgressOpponentsRobotInfo_all, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_TerritoryProgressOpponentsRobotInfo_all>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(int _1 = 0, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(TerritoryProgressOpponentsRobotInfo.ToTaskQueueHash());
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<TerritoryProgressOpponentsRobotInfo> Get(ConnectToDBPlayerService connectToDBPlayerService)
        {
            var info = await base.InternalGet(connectToDBPlayerService, 0, 0);
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(TerritoryProgressOpponentsRobotInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(0, 0, info);
        }
    }
}
