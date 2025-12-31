using Data;

namespace Script
{
    public class DbServiceProxy : ServiceProxy
    {
        public DbServiceProxy(Service self) : base(self, ServiceType.Db)
        {
        }

        public async Task<MyResponse<ResInsert_UserInfo>> InsertUserInfo(MsgInsert_UserInfo msg)
        {
            return await this.Request<MsgInsert_UserInfo, ResInsert_UserInfo>(ServiceType.Db, MsgType._Insert_UserInfo, msg);
        }

        #region auto_proxy

        public async Task<MyResponse<ResSave_AccountInfo>> Save_AccountInfo(MsgSave_AccountInfo msg)
        {
            return await this.Request<MsgSave_AccountInfo, ResSave_AccountInfo>(ServiceType.Db, MsgType._Save_AccountInfo, msg);
        }
        public async Task<MyResponse<ResQuery_AccountInfo_byElementOf_userIds>> Query_AccountInfo_byElementOf_userIds(MsgQuery_AccountInfo_byElementOf_userIds msg)
        {
            return await this.Request<MsgQuery_AccountInfo_byElementOf_userIds, ResQuery_AccountInfo_byElementOf_userIds>(ServiceType.Db, MsgType._Query_AccountInfo_byElementOf_userIds, msg);
        }
        public async Task<MyResponse<ResQuery_AccountInfo_by_channelUserId>> Query_AccountInfo_by_channelUserId(MsgQuery_AccountInfo_by_channelUserId msg)
        {
            return await this.Request<MsgQuery_AccountInfo_by_channelUserId, ResQuery_AccountInfo_by_channelUserId>(ServiceType.Db, MsgType._Query_AccountInfo_by_channelUserId, msg);
        }
        public async Task<MyResponse<ResQuery_AccountInfo_by_channel_channelUserId>> Query_AccountInfo_by_channel_channelUserId(MsgQuery_AccountInfo_by_channel_channelUserId msg)
        {
            return await this.Request<MsgQuery_AccountInfo_by_channel_channelUserId, ResQuery_AccountInfo_by_channel_channelUserId>(ServiceType.Db, MsgType._Query_AccountInfo_by_channel_channelUserId, msg);
        }
        public async Task<MyResponse<ResQuery_listOf_AccountInfo_byElementOf_userIds>> Query_listOf_AccountInfo_byElementOf_userIds(MsgQuery_listOf_AccountInfo_byElementOf_userIds msg)
        {
            return await this.Request<MsgQuery_listOf_AccountInfo_byElementOf_userIds, ResQuery_listOf_AccountInfo_byElementOf_userIds>(ServiceType.Db, MsgType._Query_listOf_AccountInfo_byElementOf_userIds, msg);
        }
        public async Task<MyResponse<ResQuery_UserInfo_by_userId>> Query_UserInfo_by_userId(MsgQuery_UserInfo_by_userId msg)
        {
            return await this.Request<MsgQuery_UserInfo_by_userId, ResQuery_UserInfo_by_userId>(ServiceType.Db, MsgType._Query_UserInfo_by_userId, msg);
        }
        public async Task<MyResponse<ResSave_UserInfo>> Save_UserInfo(MsgSave_UserInfo msg)
        {
            return await this.Request<MsgSave_UserInfo, ResSave_UserInfo>(ServiceType.Db, MsgType._Save_UserInfo, msg);
        }
        public async Task<MyResponse<ResInsert_UserInfo>> Insert_UserInfo(MsgInsert_UserInfo msg)
        {
            return await this.Request<MsgInsert_UserInfo, ResInsert_UserInfo>(ServiceType.Db, MsgType._Insert_UserInfo, msg);
        }
        public async Task<MyResponse<ResQuery_UserInfo_maxOf_userId>> Query_UserInfo_maxOf_userId(MsgQuery_UserInfo_maxOf_userId msg)
        {
            return await this.Request<MsgQuery_UserInfo_maxOf_userId, ResQuery_UserInfo_maxOf_userId>(ServiceType.Db, MsgType._Query_UserInfo_maxOf_userId, msg);
        }
        public async Task<MyResponse<ResQuery_RoomInfo_by_roomId>> Query_RoomInfo_by_roomId(MsgQuery_RoomInfo_by_roomId msg)
        {
            return await this.Request<MsgQuery_RoomInfo_by_roomId, ResQuery_RoomInfo_by_roomId>(ServiceType.Db, MsgType._Query_RoomInfo_by_roomId, msg);
        }
        public async Task<MyResponse<ResQuery_RoomInfo_maxOf_roomId>> Query_RoomInfo_maxOf_roomId(MsgQuery_RoomInfo_maxOf_roomId msg)
        {
            return await this.Request<MsgQuery_RoomInfo_maxOf_roomId, ResQuery_RoomInfo_maxOf_roomId>(ServiceType.Db, MsgType._Query_RoomInfo_maxOf_roomId, msg);
        }
        public async Task<MyResponse<ResInsert_RoomInfo>> Insert_RoomInfo(MsgInsert_RoomInfo msg)
        {
            return await this.Request<MsgInsert_RoomInfo, ResInsert_RoomInfo>(ServiceType.Db, MsgType._Insert_RoomInfo, msg);
        }
        public async Task<MyResponse<ResSave_RoomInfo>> Save_RoomInfo(MsgSave_RoomInfo msg)
        {
            return await this.Request<MsgSave_RoomInfo, ResSave_RoomInfo>(ServiceType.Db, MsgType._Save_RoomInfo, msg);
        }

        #endregion auto_proxy
    }
}