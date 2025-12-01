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
    public sealed class ProfileOriginalMailProxy : DataProxy<ProfileOriginalMail, longid, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(longid mailId, int _2 = 0)
        {
            return stDirtyElement.Create_ProfileOriginalMail(mailId);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(longid mailId, int _2 = 0)
        {
            return MailKey.OriginalMail(mailId);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ProfileOriginalMail> OnlyForSave_GetFromRedis(longid mailId)
        {
            return this.GetFromRedis(mailId, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileOriginalMail CreatePlaceholder(longid mailId, int _2 = 0)
        {
            var placeholder = new ProfileOriginalMail();
            placeholder.mailId = mailId;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(longid mailId, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileOriginalMail(mailId);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileOriginalMail)> LoadFromDB(IConnectToDBService connectToDBService, longid mailId, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileOriginalMail_by_mailId();
            msgDb.mailId = mailId;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileOriginalMail_by_mailId, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileOriginalMail_by_mailId>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(longid mailId, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileOriginalMail.ToTaskQueueHash(mailId));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileOriginalMail> Get(ConnectToDBGroupService connectToDBGroupService, longid mailId)
        {
            if (mailId == 0)
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, mailId, 0);
            if (info != null)
            {
                MyDebug.Assert(info.mailId == mailId);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileOriginalMail info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.mailId, 0, info);
        }
    }
}
