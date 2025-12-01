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
    public sealed class ProfileDeviceUidInfoProxy : DataProxy<ProfileDeviceUidInfo, string, int>
    {
        #region override

        //// AUTO CREATED ////
        protected override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        //// AUTO CREATED ////
        protected override stDirtyElement DirtyElement(string deviceUid, int _2 = 0)
        {
            return stDirtyElement.Create_ProfileDeviceUidInfo(deviceUid);
        }

        //// AUTO CREATED ////
        protected override RedisKey Key(string deviceUid, int _2 = 0)
        {
            return DeviceUidKey.Info(deviceUid);
        }

        //// AUTO CREATED ////
        protected override bool CanExpire()
        {
            return true;
        }

        //// AUTO CREATED ////
        public Task<ProfileDeviceUidInfo> OnlyForSave_GetFromRedis(string deviceUid)
        {
            return this.GetFromRedis(deviceUid, 0);
        }

        //// AUTO CREATED ////
        protected override ProfileDeviceUidInfo CreatePlaceholder(string deviceUid, int _2 = 0)
        {
            var placeholder = new ProfileDeviceUidInfo();
            placeholder.deviceUid = deviceUid;
            placeholder.SetIsPlaceholder();
            return placeholder;
        }

        //// AUTO CREATED ////
        protected override string GetLockKeyForLoadFromDBToRedis(string deviceUid, int _2 = 0)
        {
            return LockKey.LoadDataFromDBToRedis.ProfileDeviceUidInfo(deviceUid);
        }

        //// AUTO CREATED ////
        protected override async Task<(ECode, ProfileDeviceUidInfo)> LoadFromDB(IConnectToDBService connectToDBService, string deviceUid, int _2 = 0)
        {
            var msgDb = new MsgQuery_ProfileDeviceUidInfo_by_deviceUid();
            msgDb.deviceUid = deviceUid;
            MyResponse r = await connectToDBService.SendAsync(MsgType._Query_ProfileDeviceUidInfo_by_deviceUid, msgDb);
            if (r.err != ECode.Success)
            {
                return (r.err, null);
            }

            var res = r.CastRes<ResQuery_ProfileDeviceUidInfo_by_deviceUid>().result;
            return (ECode.Success, res);
        }

        //// AUTO CREATED ////
        protected override int GetBelongTaskQueue(string deviceUid, int _2 = 0)
        {
            return PersistenceTaskQueueRedis.GetQueue(ProfileDeviceUidInfo.ToTaskQueueHash(deviceUid));
        }
        #endregion override

        /////////////////////////////////////////// PUBLIC ///////////////////////////////////////////
        //// AUTO CREATED ////
        public async Task<ProfileDeviceUidInfo> Get(ConnectToDBGroupService connectToDBGroupService, string deviceUid)
        {
            if (string.IsNullOrEmpty(deviceUid))
            {
                MyDebug.Assert(false);
                return null;
            }
            var info = await base.InternalGet(connectToDBGroupService, deviceUid, 0);
            if (info != null)
            {
                MyDebug.Assert(info.deviceUid == deviceUid);
            }
            if (info != null)
            {
                info.Ensure();
            }
            return info;
        }

        //// AUTO CREATED ////
        public async Task Save(ProfileDeviceUidInfo info)
        {
            await base.SaveToRedis_Persist_IncreaseDirty(info.deviceUid, 0, info);
        }
    }
}
