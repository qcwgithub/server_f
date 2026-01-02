using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        void InitDict()
        {
            var dict = this.configDict;

            #region auto_init

            dict[MsgType._Service_ReloadScript] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_CheckConnections_Loop] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_ConnectorInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GetPendingMessageList] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GetScriptVersion] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_ReloadConfigs] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GC] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_RemoteWillShutdown] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GetServiceState] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_PersistenceTaskQueueHandler] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_PersistenceTaskQueueHandler_Loop] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GetReloadConfigOptions] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_GetConnectedInfos] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_ViewMongoDumpList] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Service_A_ResGetServiceConfigs] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Global_GetServiceConfigs] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Gateway_ServerAction] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Gateway_ServerKick] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._User_ServerAction] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._User_UserLoginSuccess] = new stMessageConfig
            {
                queue = MessageQueue.User,
            };

            dict[MsgType._User_ServerKick] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._User_UserDisconnectFromGateway] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._User_SaveUserImmediately] = new stMessageConfig
            {
                queue = MessageQueue.User,
            };

            dict[MsgType._User_GetUserCount] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._User_SaveUserInfoToFile] = new stMessageConfig
            {
                queue = MessageQueue.User,
            };

            dict[MsgType._User_SetGmFlag] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._UserManager_UserLogin] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Room_ServerAction] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Room_SaveRoomInfoToFile] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Room_UserEnter] = new stMessageConfig
            {
                queue = MessageQueue.Room,
            };

            dict[MsgType._Room_UserLeave] = new stMessageConfig
            {
                queue = MessageQueue.Room,
            };

            dict[MsgType._Room_LoadRoom] = new stMessageConfig
            {
                queue = MessageQueue.Room,
            };

            dict[MsgType._Room_SaveRoomImmediately] = new stMessageConfig
            {
                queue = MessageQueue.Room,
            };

            dict[MsgType._RoomManager_LoadRoom] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformReloadScript] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformSaveUserInfoToFile] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformShowScriptVersion] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformGetPendingMsgList] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformShutdown] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformPlayerGM] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformKick] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Command_PerformSetPlayerGmFlag] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Save_AccountInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_AccountInfo_byElementOf_userIds] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_AccountInfo_by_channelUserId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_AccountInfo_by_channel_channelUserId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_listOf_AccountInfo_byElementOf_userIds] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_UserInfo_by_userId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Save_UserInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Insert_UserInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_UserInfo_maxOf_userId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_RoomInfo_by_roomId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Query_RoomInfo_maxOf_roomId] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Insert_RoomInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType._Save_RoomInfo] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType.ClientStart] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType.Login] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType.Kick] = new stMessageConfig
            {
                queue = MessageQueue.None,
            };

            dict[MsgType.EnterRoom] = new stMessageConfig
            {
                queue = MessageQueue.User,
            };

            dict[MsgType.LeaveRoom] = new stMessageConfig
            {
                queue = MessageQueue.User,
            };

            #endregion auto_init
        }
    }
}