using MessagePack;

namespace Data
{
    public partial class MsgConfigData
    {
        void Init()
        {
            var dict = this.configDict;

            #region auto_init

            dict[MsgType._Service_Start] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_Shutdown] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_OnConnectComplete] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_OnConnectionClose] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_ReloadScript] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_CheckConnections] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_CheckConnections_Loop] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_ConnectorInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GetPendingMessageList] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GetScriptVersion] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_OnHttpRequest] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_ReloadConfigs] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GC] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_RemoteWillShutdown] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_WaitTask] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GetServiceState] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_PersistenceTaskQueueHandler] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_PersistenceTaskQueueHandler_Loop] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GetReloadConfigOptions] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_GetConnectedInfos] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_ViewMongoDumpList] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Service_A_ResGetServiceConfigs] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Global_GetServiceConfigs] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Gateway_ServerAction] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Gateway_DestroyUser] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Gateway_ServerKick] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._User_ServerAction] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._User_UserLoginSuccess] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType._User_ServerKick] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._User_UserDisconnectFromGateway] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._User_DestroyUser] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType._User_SaveUserImmediately] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType._User_GetUserCount] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._User_SaveUserInfoToFile] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType._User_SaveUser] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType._User_SetGmFlag] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._UserManager_UserLogin] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Room_ServerAction] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Room_DestroyRoom] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._Room_SaveRoom] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._Room_SaveRoomInfoToFile] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Room_UserEnter] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._Room_UserLeave] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._Room_LoadRoom] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._Room_SaveRoomImmediately] = new stMsgConfig
            {
                queue = MsgQueue.Room,
            };

            dict[MsgType._RoomManager_LoadRoom] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformReloadScript] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformSaveUserInfoToFile] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformShowScriptVersion] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformGetPendingMsgList] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformShutdown] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformPlayerGM] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformKick] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Command_PerformSetPlayerGmFlag] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Save_AccountInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_AccountInfo_byElementOf_userIds] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_AccountInfo_by_channelUserId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_AccountInfo_by_channel_channelUserId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_listOf_AccountInfo_byElementOf_userIds] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_UserInfo_by_userId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Save_UserInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Insert_UserInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_UserInfo_maxOf_userId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_RoomInfo_by_roomId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Query_RoomInfo_maxOf_roomId] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Insert_RoomInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType._Save_RoomInfo] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType.ClientStart] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType.Login] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType.Kick] = new stMsgConfig
            {
                queue = MsgQueue.None,
            };

            dict[MsgType.EnterRoom] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            dict[MsgType.LeaveRoom] = new stMsgConfig
            {
                queue = MsgQueue.User,
            };

            #endregion auto_init
        }

        public object DeserializeMsg(MsgType msgType, ArraySegment<byte> msg)
        {
            switch (msgType)
            {
                #region auto_deserialize

                case MsgType._Service_Start:
                    return MessagePackSerializer.Deserialize<MsgStart>(msg);

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Deserialize<MsgShutdown>(msg);

                case MsgType._Service_OnConnectComplete:
                    return MessagePackSerializer.Deserialize<MsgOnConnectComplete>(msg);

                case MsgType._Service_OnConnectionClose:
                    return MessagePackSerializer.Deserialize<MsgConnectionClose>(msg);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Deserialize<MsgReloadScript>(msg);

                case MsgType._Service_CheckConnections:
                    return MessagePackSerializer.Deserialize<MsgCheckConnections>(msg);

                case MsgType._Service_CheckConnections_Loop:
                    return MessagePackSerializer.Deserialize<MsgCheckConnections_Loop>(msg);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Deserialize<MsgConnectorInfo>(msg);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Deserialize<MsgGetPendingMsgList>(msg);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Deserialize<MsgGetScriptVersion>(msg);

                case MsgType._Service_OnHttpRequest:
                    return MessagePackSerializer.Deserialize<MsgOnHttpRequest>(msg);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Deserialize<MsgReloadConfigs>(msg);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Deserialize<MsgGC>(msg);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Deserialize<MsgRemoteWillShutdown>(msg);

                case MsgType._Service_WaitTask:
                    return MessagePackSerializer.Deserialize<MsgWaitTask>(msg);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Deserialize<MsgGetServiceState>(msg);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler");

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Deserialize<MsgGetReloadConfigOptions>(msg);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Deserialize<MsgGetConnectedInfos>(msg);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Deserialize<MsgViewMongoDumpList>(msg);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Deserialize<A_ResGetServiceConfigs>(msg);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Deserialize<MsgGetServiceConfigs>(msg);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgGatewayServiceAction>(msg);

                case MsgType._Gateway_DestroyUser:
                    return MessagePackSerializer.Deserialize<MsgGatewayDestroyUser>(msg);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Deserialize<MsgGatewayServerKick>(msg);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgUserServiceAction>(msg);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Deserialize<MsgUserLoginSuccess>(msg);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Deserialize<MsgUserServerKick>(msg);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Deserialize<MsgUserDisconnectFromGateway>(msg);

                case MsgType._User_DestroyUser:
                    return MessagePackSerializer.Deserialize<MsgUserDestroyUser>(msg);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Deserialize<MsgSaveUser>(msg);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Deserialize<MsgGetUserCount>(msg);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgSaveUserInfoToFile>(msg);

                case MsgType._User_SaveUser:
                    return MessagePackSerializer.Deserialize<MsgSaveUser>(msg);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Deserialize<MsgSetGmFlag>(msg);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Deserialize<MsgUserManagerUserLogin>(msg);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgRoomServiceAction>(msg);

                case MsgType._Room_DestroyRoom:
                    return MessagePackSerializer.Deserialize<MsgRoomDestroyRoom>(msg);

                case MsgType._Room_SaveRoom:
                    return MessagePackSerializer.Deserialize<MsgSaveRoom>(msg);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgSaveRoomInfoToFile>(msg);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Deserialize<MsgRoomUserEnter>(msg);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Deserialize<MsgRoomUserLeave>(msg);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Deserialize<MsgRoomLoadRoom>(msg);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Deserialize<MsgSaveRoom>(msg);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Deserialize<MsgRoomManagerLoadRoom>(msg);

                case MsgType._Command_PerformReloadScript:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformSaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformShowScriptVersion:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformGetPendingMsgList:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformShutdown:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformPlayerGM:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformKick:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Command_PerformSetPlayerGmFlag:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msg);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_AccountInfo>(msg);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_byElementOf_userIds>(msg);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channelUserId>(msg);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channel_channelUserId>(msg);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<MsgQuery_listOf_AccountInfo_byElementOf_userIds>(msg);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Deserialize<MsgQuery_UserInfo_by_userId>(msg);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_UserInfo>(msg);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Deserialize<MsgInsert_UserInfo>(msg);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Deserialize<MsgQuery_UserInfo_maxOf_userId>(msg);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_by_roomId>(msg);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_maxOf_roomId>(msg);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Deserialize<MsgInsert_RoomInfo>(msg);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_RoomInfo>(msg);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Deserialize<MsgLogin>(msg);

                case MsgType.Kick:
                    return MessagePackSerializer.Deserialize<MsgKick>(msg);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Deserialize<MsgEnterRoom>(msg);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Deserialize<MsgLeaveRoom>(msg);

                #endregion auto_deserialize

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}