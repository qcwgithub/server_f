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
        public async Task<MyResponse> Query_SceneInfo_by_sceneId(MsgQuery_SceneInfo_by_sceneId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_SceneInfo_by_sceneId, msg);
        }
        public async Task<MyResponse> Query_SceneInfo_maxOf_sceneId(MsgQuery_SceneInfo_maxOf_sceneId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_SceneInfo_maxOf_sceneId, msg);
        }
        public async Task<MyResponse> Insert_SceneInfo(MsgInsert_SceneInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Insert_SceneInfo, msg);
        }
        public async Task<MyResponse> Save_SceneInfo(MsgSave_SceneInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_SceneInfo, msg);
        }
        public async Task<MyResponse> Search_SceneInfo(MsgSearch_SceneInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Search_SceneInfo, msg);
        }
        public async Task<MyResponse> Save_MessageReportInfo(MsgSave_MessageReportInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_MessageReportInfo, msg);
        }
        public async Task<MyResponse> Save_UserReportInfo(MsgSave_UserReportInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_UserReportInfo, msg);
        }
        public async Task<MyResponse> Save_UserBriefInfo(MsgSave_UserBriefInfo msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Save_UserBriefInfo, msg);
        }
        public async Task<MyResponse> Query_UserBriefInfo_by_userId(MsgQuery_UserBriefInfo_by_userId msg)
        {
            return await this.Request(ServiceType.Db, MsgType._Query_UserBriefInfo_by_userId, msg);
        }

        #endregion auto_proxy
    }
}