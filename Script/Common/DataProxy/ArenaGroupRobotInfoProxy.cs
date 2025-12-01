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
    public sealed class ArenaGroupRobotInfoProxy : DataProxy<ArenaGroupRobotInfo, int, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(int groupType, int groupId)
        {
            return stDirtyElement.Create_ArenaGroupRobotInfo(groupType, groupId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(int groupType, int groupId)
        {
            return ArenaKey.GroupRobotInfo(groupType, groupId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return false;
        }

        //// AUTO CREATED ////
        public Task<ArenaGroupRobotInfo> OnlyForSave_GetFromRedis(int groupType, int groupId)
        {
            return this.GetFromRedis(groupType, groupId);
        }

        //// AUTO CREATED ////
        protected override ArenaGroupRobotInfo CreatePlaceholder(int groupType, int groupId)
        {
            var placeholder = new ArenaGroupRobotInfo();
            placeholder.groupType = groupType;
            placeholder.groupId = groupId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(int groupType, int groupId)
        {
            return LockKey.LoadDataFromDBToRedis.ArenaGroupRobotInfo(groupType, groupId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ArenaGroupRobotInfo)> LoadFromDB(IConnectToDBService connectToDBService, int groupType, int groupId)
        {
            var msgDb = new MsgQuery_ArenaGroupRobotInfo_by_groupType_groupId();
            msgDb.groupType = groupType;
            msgDb.groupId = groupId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ArenaGroupRobotInfo_by_groupType_groupId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ArenaGroupRobotInfo_by_groupType_groupId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(int groupType, int groupId)
        {
            return PersistenceTaskQueueRedis.GetQueue(ArenaGroupRobotInfo.ToTaskQueueHash(groupType, groupId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ArenaGroupRobotInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, int groupType, int groupId)
        {
            if (groupType == 0 && groupId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, groupType, groupId);
            if (info != null)
            {
                MyDebug.Assert(info.groupType == groupType);
                MyDebug.Assert(info.groupId == groupId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ArenaGroupRobotInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.groupType, info.groupId, info);
        }
    }
}
