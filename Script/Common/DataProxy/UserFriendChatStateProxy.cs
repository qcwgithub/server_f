using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;

namespace Script
{
    //// AUTO CREATED ////
    public partial class UserFriendChatStateProxy : DataProxy<UserFriendChatState, long, int>
    {
        //// AUTO CREATED ////
        public UserFriendChatStateProxy(Server server) : base(server)
        {
        }

        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(long userId, int _2 = 0)
        {
            return stDirtyElement.Create_UserFriendChatState(userId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(long userId, int _2 = 0)
        {
            return UserKey.FriendChatState(userId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        protected override string RedisValueFormat()
        {
            return "hash";
        }

        //// AUTO CREATED ////
        public Task<UserFriendChatState> OnlyForSave_GetFromRedis(long userId)
        {
            return this.GetFromRedis(userId, 0);
        }

        //// AUTO CREATED ////
        protected override UserFriendChatState CreatePlaceholder(long userId, int _2 = 0)
        {
            var placeholder = new UserFriendChatState();
            placeholder.userId = userId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(long userId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.UserFriendChatState(userId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, UserFriendChatState)> LoadFromDB(DbServiceProxy dbServiceProxy, long userId, int _2 = 0)
        {
            var msgDb = new MsgQuery_UserFriendChatState_by_userId();
            msgDb.userId = userId;
            var r = await dbServiceProxy.Query_UserFriendChatState_by_userId(msgDb);
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResQuery_UserFriendChatState_by_userId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(long userId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(userId.GetHashCode());
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<UserFriendChatState> Get(DbServiceProxy DbServiceProxy, long userId)
        {
            if (userId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(DbServiceProxy, userId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.userId == userId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(UserFriendChatState info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.userId, 0, info);
        }
    }
}
