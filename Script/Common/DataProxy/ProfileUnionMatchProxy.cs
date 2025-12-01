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
    public sealed class ProfileUnionMatchProxy : DataProxy<ProfileUnionMatch, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid matchId, int _2 = 0)
        {
            return stDirtyElement.Create_ProfileUnionMatch(matchId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid matchId, int _2 = 0)
        {
            return UnionCKey.Match(matchId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ProfileUnionMatch> OnlyForSave_GetFromRedis(longid matchId)
        {
            return this.GetFromRedis(matchId, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileUnionMatch CreatePlaceholder(longid matchId, int _2 = 0)
        {
            var placeholder = new ProfileUnionMatch();
            placeholder.matchId = matchId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid matchId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileUnionMatch(matchId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileUnionMatch)> LoadFromDB(IConnectToDBService connectToDBService, longid matchId, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileUnionMatch_by_matchId();
            msgDb.matchId = matchId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileUnionMatch_by_matchId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileUnionMatch_by_matchId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid matchId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileUnionMatch.ToTaskQueueHash(matchId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileUnionMatch> Get(ConnectToDBGroupService connectToDBGroupService, longid matchId)
        {
            if (matchId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, matchId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.matchId == matchId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileUnionMatch info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.matchId, 0, info);
        }
    }
}
