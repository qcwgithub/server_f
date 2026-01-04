using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class AccountInfoProxy : DataProxy<AccountInfo, string, string>
    {
        //// AUTO CREATED ////
        public AccountInfoProxy(Server server) : base(server)
        {
        }

        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(string channel, string channelUserId)
        {
            return stDirtyElement.Create_AccountInfo(channel, channelUserId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(string channel, string channelUserId)
        {
            return AccountKey.AccountInfo(channel, channelUserId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<AccountInfo> OnlyForSave_GetFromRedis(string channel, string channelUserId)
        {
            return this.GetFromRedis(channel, channelUserId);
        }

        //// AUTO CREATED ////
        protected override AccountInfo CreatePlaceholder(string channel, string channelUserId)
        {
            var placeholder = new AccountInfo();
            placeholder.channel = channel;
            placeholder.channelUserId = channelUserId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(string channel, string channelUserId)
        {
            return LockKey.LoadDataFromDBToRedis.AccountInfo(channel, channelUserId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, AccountInfo)> LoadFromDB(DbServiceProxy dbServiceProxy, string channel, string channelUserId)
        {
            var msgDb = new MsgQuery_AccountInfo_by_channel_channelUserId();
            msgDb.channel = channel;
            msgDb.channelUserId = channelUserId;
            var r = await dbServiceProxy.Query_AccountInfo_by_channel_channelUserId(msgDb);
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResQuery_AccountInfo_by_channel_channelUserId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(string channel, string channelUserId)
        {
            return PersistenceTaskQueueRedis.GetQueue(AccountInfo.ToTaskQueueHash(channelUserId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<AccountInfo> Get(DbServiceProxy DbServiceProxy, string channel, string channelUserId)
        {
            if (string.IsNullOrEmpty(channel) && string.IsNullOrEmpty(channelUserId))
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(DbServiceProxy, channel, channelUserId);
            if (info != null)
            {
                MyDebug.Assert(info.channel == channel);
                MyDebug.Assert(info.channelUserId == channelUserId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(AccountInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.channel, info.channelUserId, info);
        }
    }
}
