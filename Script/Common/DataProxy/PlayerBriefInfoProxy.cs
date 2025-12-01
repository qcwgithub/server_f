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
    public partial class PlayerBriefInfoProxy : DataProxy<PlayerBriefInfo, longid, int>
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
            return stDirtyElement.Create_PlayerBriefInfo(playerId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid playerId, int _2 = 0)
        {
            return PlayerKey.Brief(playerId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<PlayerBriefInfo> OnlyForSave_GetFromRedis(longid playerId)
        {
            return this.GetFromRedis(playerId, 0);
        }

        //// AUTO CREATED ////
        protected override PlayerBriefInfo CreatePlaceholder(longid playerId, int _2 = 0)
        {
            throw new NotImplementedException();
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid playerId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.PlayerBriefInfo(playerId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, PlayerBriefInfo)> LoadFromDB(IConnectToDBService connectToDBService, longid playerId, int _2 = 0)
        {
            var msgDb = new MsgQuery_PlayerBriefInfo_by_playerId();
            msgDb.playerId = playerId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_PlayerBriefInfo_by_playerId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_PlayerBriefInfo_by_playerId>().result;
            if (res == null)
            {
                res = new PlayerBriefInfo().CreateDefaultForRedis(playerId);
            }
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid playerId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(PlayerBriefInfo.ToTaskQueueHash(playerId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<PlayerBriefInfo> Get(ConnectToDBPlayerService connectToDBPlayerService, longid playerId)
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
        public async Task Save(PlayerBriefInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.playerId, 0, info);
        }
    }
}
