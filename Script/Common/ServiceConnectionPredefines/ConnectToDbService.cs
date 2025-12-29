using Data;

namespace Script
{
    public class ConnectToDbService : ConnectToStatelessService
    {
        public ConnectToDbService(Service self) : base(self, ServiceType.Db)
        {

        }

        public async Task<MyResponse<ResInsert_UserInfo>> InsertUserInfo(MsgInsert_UserInfo msg)
        {
            return await this.Request<MsgInsert_UserInfo, ResInsert_UserInfo>(MsgType._Insert_UserInfo, msg);
        }

        public async Task<MyResponse<ResQuery_UserInfo_maxOf_userId>> Query_UserInfo_maxOf_userId(MsgQuery_UserInfo_maxOf_userId msg)
        {
            return await this.Request<MsgQuery_UserInfo_maxOf_userId, ResQuery_UserInfo_maxOf_userId>(MsgType._Query_UserInfo_maxOf_userId, msg);
        }

        public async Task<MyResponse<ResSave_UserInfo>> Save_UserInfo(MsgSave_UserInfo msg)
        {
            return await this.Request<MsgSave_UserInfo, ResSave_UserInfo>(MsgType._Save_UserInfo, msg);
        }

        public async Task<MyResponse<ResQuery_UserInfo_by_userId>> Query_UserInfo_by_userId(MsgQuery_UserInfo_by_userId msg)
        {
            return await this.Request<MsgQuery_UserInfo_by_userId, ResQuery_UserInfo_by_userId>(MsgType._Query_UserInfo_by_userId, msg);
        }

        public async Task<MyResponse<ResInsert_RoomInfo>> Insert_RoomInfo(MsgInsert_RoomInfo msg)
        {
            return await this.Request<MsgInsert_RoomInfo, ResInsert_RoomInfo>(MsgType._Insert_RoomInfo, msg);
        }

        public async Task<MyResponse<ResQuery_RoomInfo_by_roomId>> Query_RoomInfo_by_roomId(MsgQuery_RoomInfo_by_roomId msg)
        {
            return await this.Request<MsgQuery_RoomInfo_by_roomId, ResQuery_RoomInfo_by_roomId>(MsgType._Query_RoomInfo_by_roomId, msg);
        }

        public async Task<MyResponse<ResSave_RoomInfo>> Save_RoomInfo(MsgSave_RoomInfo msg)
        {
            return await this.Request<MsgSave_RoomInfo, ResSave_RoomInfo>(MsgType._Save_RoomInfo, msg);
        }
    }
}