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
    public sealed class UnionSeasonInfoDProxy : DataProxy<UnionSeasonInfoD, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid unionId, int _2 = 0)
        {
            return stDirtyElement.Create_UnionSeasonInfoD(unionId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid unionId, int _2 = 0)
        {
            return UnionKey.SeasonInfoD(unionId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<UnionSeasonInfoD> OnlyForSave_GetFromRedis(longid unionId)
        {
            return this.GetFromRedis(unionId, 0);
        }

        //// AUTO CREATED ////
        protected override UnionSeasonInfoD CreatePlaceholder(longid unionId, int _2 = 0)
        {
            var placeholder = new UnionSeasonInfoD();
            placeholder.unionId = unionId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid unionId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.UnionSeasonInfoD(unionId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, UnionSeasonInfoD)> LoadFromDB(IConnectToDBService connectToDBService, longid unionId, int _2 = 0)
        {
            var msgDb = new MsgQuery_UnionSeasonInfoD_by_unionId();
            msgDb.unionId = unionId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_UnionSeasonInfoD_by_unionId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_UnionSeasonInfoD_by_unionId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid unionId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(UnionSeasonInfoD.ToTaskQueueHash(unionId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<UnionSeasonInfoD> Get(ConnectToDBPlayerService connectToDBPlayerService, longid unionId)
        {
            if (unionId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, unionId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.unionId == unionId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(UnionSeasonInfoD info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.unionId, 0, info);
        }
    }
}
