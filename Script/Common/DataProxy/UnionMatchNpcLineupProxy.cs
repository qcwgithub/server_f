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
    public sealed class UnionMatchNpcLineupProxy : DataProxy<UnionMatchNpcLineup, string, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(string npcId, int _2 = 0)
        {
            return stDirtyElement.Create_UnionMatchNpcLineup(npcId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(string npcId, int _2 = 0)
        {
            return UnionCKey.NpcLineup(npcId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<UnionMatchNpcLineup> OnlyForSave_GetFromRedis(string npcId)
        {
            return this.GetFromRedis(npcId, 0);
        }

        //// AUTO CREATED ////
        protected override UnionMatchNpcLineup CreatePlaceholder(string npcId, int _2 = 0)
        {
            var placeholder = new UnionMatchNpcLineup();
            placeholder.npcId = npcId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(string npcId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.UnionMatchNpcLineup(npcId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, UnionMatchNpcLineup)> LoadFromDB(IConnectToDBService connectToDBService, string npcId, int _2 = 0)
        {
            var msgDb = new MsgQuery_UnionMatchNpcLineup_by_npcId();
            msgDb.npcId = npcId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_UnionMatchNpcLineup_by_npcId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_UnionMatchNpcLineup_by_npcId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(string npcId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(UnionMatchNpcLineup.ToTaskQueueHash(npcId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<UnionMatchNpcLineup> Get(ConnectToDBGroupService connectToDBGroupService, string npcId)
        {
            if (string.IsNullOrEmpty(npcId))
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, npcId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.npcId == npcId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(UnionMatchNpcLineup info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.npcId, 0, info);
        }
    }
}
