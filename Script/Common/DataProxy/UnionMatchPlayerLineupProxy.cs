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
    public sealed class UnionMatchPlayerLineupProxy : DataProxy<UnionMatchPlayerLineup, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid playerId, int lineupIndex)
        {
            return stDirtyElement.Create_UnionMatchPlayerLineup(playerId, lineupIndex);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid playerId, int lineupIndex)
        {
            return UnionCKey.PlayerLineup(playerId, lineupIndex);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<UnionMatchPlayerLineup> OnlyForSave_GetFromRedis(longid playerId, int lineupIndex)
        {
            return this.GetFromRedis(playerId, lineupIndex);
        }

        //// AUTO CREATED ////
        protected override UnionMatchPlayerLineup CreatePlaceholder(longid playerId, int lineupIndex)
        {
            var placeholder = new UnionMatchPlayerLineup();
            placeholder.playerId = playerId;
            placeholder.lineupIndex = lineupIndex;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid playerId, int lineupIndex)
        {
            return LockKey.LoadDataFromDBToRedis.UnionMatchPlayerLineup(playerId, lineupIndex);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, UnionMatchPlayerLineup)> LoadFromDB(IConnectToDBService connectToDBService, longid playerId, int lineupIndex)
        {
            var msgDb = new MsgQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex();
            msgDb.playerId = playerId;
            msgDb.lineupIndex = lineupIndex;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_UnionMatchPlayerLineup_by_playerId_lineupIndex, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid playerId, int lineupIndex)
        {
            return PersistenceTaskQueueRedis.GetQueue(UnionMatchPlayerLineup.ToTaskQueueHash(playerId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<UnionMatchPlayerLineup> Get(ConnectToDBGroupService connectToDBGroupService, longid playerId, int lineupIndex)
        {
            if (playerId == 0 && lineupIndex == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, playerId, lineupIndex);
            if (info != null)
            {
                MyDebug.Assert(info.playerId == playerId);
                MyDebug.Assert(info.lineupIndex == lineupIndex);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(UnionMatchPlayerLineup info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.playerId, info.lineupIndex, info);
        }
    }
}
