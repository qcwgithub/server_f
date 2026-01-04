using Data;

namespace Script
{
    public class DbServiceProxy : ServiceProxy
    {
        public DbServiceProxy(Service self) : base(self, ServiceType.Db)
        {
        }

        public async Task<MyResponse> InsertUserInfo(MsgInsert_UserInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Insert_UserInfo, msg);
        }

        #region auto_proxy

        public async Task<MyResponse> Save_AccountInfo(MsgSave_AccountInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_AccountInfo, msg);
        }
        public async Task<MyResponse> Query_AccountInfo_byElementOf_userIds(MsgQuery_AccountInfo_byElementOf_userIds msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_AccountInfo_byElementOf_userIds, msg);
        }
        public async Task<MyResponse> Query_AccountInfo_by_channelUserId(MsgQuery_AccountInfo_by_channelUserId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_AccountInfo_by_channelUserId, msg);
        }
        public async Task<MyResponse> Query_AccountInfo_by_channel_channelUserId(MsgQuery_AccountInfo_by_channel_channelUserId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_AccountInfo_by_channel_channelUserId, msg);
        }
        public async Task<MyResponse> Query_listOf_AccountInfo_byElementOf_userIds(MsgQuery_listOf_AccountInfo_byElementOf_userIds msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_listOf_AccountInfo_byElementOf_userIds, msg);
        }
        public async Task<MyResponse> Query_UserInfo_by_userId(MsgQuery_UserInfo_by_userId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_UserInfo_by_userId, msg);
        }
        public async Task<MyResponse> Save_UserInfo(MsgSave_UserInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_UserInfo, msg);
        }
        public async Task<MyResponse> Insert_UserInfo(MsgInsert_UserInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Insert_UserInfo, msg);
        }
        public async Task<MyResponse> Query_UserInfo_maxOf_userId(MsgQuery_UserInfo_maxOf_userId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_UserInfo_maxOf_userId, msg);
        }
        public async Task<MyResponse> Query_RoomInfo_by_roomId(MsgQuery_RoomInfo_by_roomId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_RoomInfo_by_roomId, msg);
        }
        public async Task<MyResponse> Query_RoomInfo_maxOf_roomId(MsgQuery_RoomInfo_maxOf_roomId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_RoomInfo_maxOf_roomId, msg);
        }
        public async Task<MyResponse> Insert_RoomInfo(MsgInsert_RoomInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Insert_RoomInfo, msg);
        }
        public async Task<MyResponse> Save_RoomInfo(MsgSave_RoomInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_RoomInfo, msg);
        }

        #endregion auto_proxy
    }
}