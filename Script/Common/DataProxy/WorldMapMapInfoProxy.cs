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
    public sealed class WorldMapMapInfoProxy : DataProxy<WorldMapMapInfo, string, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(string mapId, int _2 = 0)
        {
            return stDirtyElement.Create_WorldMapMapInfo(mapId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(string mapId, int _2 = 0)
        {
            return WorldMapKey.MapInfo(mapId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<WorldMapMapInfo> OnlyForSave_GetFromRedis(string mapId)
        {
            return this.GetFromRedis(mapId, 0);
        }

        //// AUTO CREATED ////
        protected override WorldMapMapInfo CreatePlaceholder(string mapId, int _2 = 0)
        {
            var placeholder = new WorldMapMapInfo();
            placeholder.mapId = mapId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(string mapId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.WorldMapMapInfo(mapId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, WorldMapMapInfo)> LoadFromDB(IConnectToDBService connectToDBService, string mapId, int _2 = 0)
        {
            var msgDb = new MsgQuery_WorldMapMapInfo_by_mapId();
            msgDb.mapId = mapId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_WorldMapMapInfo_by_mapId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_WorldMapMapInfo_by_mapId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(string mapId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(WorldMapMapInfo.ToTaskQueueHash(mapId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<WorldMapMapInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, string mapId)
        {
            if (string.IsNullOrEmpty(mapId))
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, mapId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.mapId == mapId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(WorldMapMapInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.mapId, 0, info);
        }
    }
}
