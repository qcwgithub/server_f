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
    public sealed class ExpeditionPartyPlayerInfoProxy : DataProxy<ExpeditionPartyPlayerInfo, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid playerId, int _2 = 0)
        {
            return stDirtyElement.Create_ExpeditionPartyPlayerInfo(playerId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid playerId, int _2 = 0)
        {
            return ExpeditionPartyKey.Player.Info(playerId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ExpeditionPartyPlayerInfo> OnlyForSave_GetFromRedis(longid playerId)
        {
            return this.GetFromRedis(playerId, 0);
        }

        //// AUTO CREATED ////
        protected override ExpeditionPartyPlayerInfo CreatePlaceholder(longid playerId, int _2 = 0)
        {
            var placeholder = new ExpeditionPartyPlayerInfo();
            placeholder.playerId = playerId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid playerId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ExpeditionPartyPlayerInfo(playerId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ExpeditionPartyPlayerInfo)> LoadFromDB(IConnectToDBService connectToDBService, longid playerId, int _2 = 0)
        {
            var msgDb = new MsgQuery_ExpeditionPartyPlayerInfo_by_playerId();
            msgDb.playerId = playerId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ExpeditionPartyPlayerInfo_by_playerId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ExpeditionPartyPlayerInfo_by_playerId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid playerId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ExpeditionPartyPlayerInfo.ToTaskQueueHash(playerId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ExpeditionPartyPlayerInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, longid playerId)
        {
            if (playerId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, playerId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.playerId == playerId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ExpeditionPartyPlayerInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.playerId, 0, info);
        }
    }
}
