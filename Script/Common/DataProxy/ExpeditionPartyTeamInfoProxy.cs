using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;
using longid = System.Int64;
using ExpeditionPartyTeamInfo = Data.TeamInfo;


namespace Script
{
    //// AUTO CREATED ////
    public sealed class ExpeditionPartyTeamInfoProxy : DataProxy<ExpeditionPartyTeamInfo, long, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(long teamId, int _2 = 0)
        {
            return stDirtyElement.Create_ExpeditionPartyTeamInfo(teamId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(long teamId, int _2 = 0)
        {
            return ExpeditionPartyKey.Team.Info(teamId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ExpeditionPartyTeamInfo> OnlyForSave_GetFromRedis(long teamId)
        {
            return this.GetFromRedis(teamId, 0);
        }

        //// AUTO CREATED ////
        protected override ExpeditionPartyTeamInfo CreatePlaceholder(long teamId, int _2 = 0)
        {
            var placeholder = new ExpeditionPartyTeamInfo();
            placeholder.teamId = teamId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(long teamId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ExpeditionPartyTeamInfo(teamId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ExpeditionPartyTeamInfo)> LoadFromDB(IConnectToDBService connectToDBService, long teamId, int _2 = 0)
        {
            var msgDb = new MsgQuery_ExpeditionPartyTeamInfo_by_teamId();
            msgDb.teamId = teamId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ExpeditionPartyTeamInfo_by_teamId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ExpeditionPartyTeamInfo_by_teamId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(long teamId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ExpeditionPartyTeamInfo.ToTaskQueueHash(teamId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ExpeditionPartyTeamInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, long teamId)
        {
            if (teamId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBPlayerService, teamId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.teamId == teamId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ExpeditionPartyTeamInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.teamId, 0, info);
        }
    }
}
