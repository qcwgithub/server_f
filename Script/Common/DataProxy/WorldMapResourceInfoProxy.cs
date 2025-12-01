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
    public sealed class WorldMapResourceInfoProxy : DataProxy<WorldMapResourceInfo, string, string>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(string mapId, string resourceId)
        {
            return stDirtyElement.Create_WorldMapResourceInfo(mapId, resourceId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(string mapId, string resourceId)
        {
            return WorldMapKey.Resource(mapId, resourceId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<WorldMapResourceInfo> OnlyForSave_GetFromRedis(string mapId, string resourceId)
        {
            return this.GetFromRedis(mapId, resourceId);
        }

        //// AUTO CREATED ////
        protected override WorldMapResourceInfo CreatePlaceholder(string mapId, string resourceId)
        {
            var placeholder = new WorldMapResourceInfo();
            placeholder.mapId = mapId;
            placeholder.resourceId = resourceId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(string mapId, string resourceId)
        {
            return LockKey.LoadDataFromDBToRedis.WorldMapResourceInfo(mapId, resourceId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, WorldMapResourceInfo)> LoadFromDB(IConnectToDBService connectToDBService, string mapId, string resourceId)
        {
            var msgDb = new MsgQuery_WorldMapResourceInfo_by_mapId_resourceId();
            msgDb.mapId = mapId;
            msgDb.resourceId = resourceId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_WorldMapResourceInfo_by_mapId_resourceId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_WorldMapResourceInfo_by_mapId_resourceId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(string mapId, string resourceId)
        {
            return PersistenceTaskQueueRedis.GetQueue(WorldMapResourceInfo.ToTaskQueueHash(resourceId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<WorldMapResourceInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, string mapId, string resourceId)
        {
            if (string.IsNullOrEmpty(mapId) && string.IsNullOrEmpty(resourceId))
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, mapId, resourceId);
            if (info != null)
            {
                MyDebug.Assert(info.mapId == mapId);
                MyDebug.Assert(info.resourceId == resourceId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(WorldMapResourceInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.mapId, info.resourceId, info);
        }
    }
}
