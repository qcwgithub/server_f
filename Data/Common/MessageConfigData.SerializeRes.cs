using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public byte[] SerializeRes(MsgType msgType, object res)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Start:
                    return MessagePackSerializer.Serialize<MsgStart>((MsgStart)res);

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Serialize<MsgShutdown>((MsgShutdown)res);

                case MsgType._Service_OnConnectComplete:
                    return MessagePackSerializer.Serialize<MsgOnConnectComplete>((MsgOnConnectComplete)res);

                case MsgType._Service_OnConnectionClose:
                    return MessagePackSerializer.Serialize<MsgConnectionClose>((MsgConnectionClose)res);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Serialize<MsgReloadScript>((MsgReloadScript)res);

                case MsgType._Service_CheckConnections:
                    return MessagePackSerializer.Serialize<MsgCheckConnections>((MsgCheckConnections)res);

                case MsgType._Service_CheckConnections_Loop:
                    return MessagePackSerializer.Serialize<MsgCheckConnections_Loop>((MsgCheckConnections_Loop)res);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Serialize<MsgConnectorInfo>((MsgConnectorInfo)res);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Serialize<MsgGetPendingMsgList>((MsgGetPendingMsgList)res);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Serialize<MsgGetScriptVersion>((MsgGetScriptVersion)res);

                case MsgType._Service_OnHttpRequest:
                    return MessagePackSerializer.Serialize<MsgOnHttpRequest>((MsgOnHttpRequest)res);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Serialize<MsgReloadConfigs>((MsgReloadConfigs)res);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Serialize<MsgGC>((MsgGC)res);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Serialize<MsgRemoteWillShutdown>((MsgRemoteWillShutdown)res);

                case MsgType._Service_WaitTask:
                    return MessagePackSerializer.Serialize<MsgWaitTask>((MsgWaitTask)res);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Serialize<MsgGetServiceState>((MsgGetServiceState)res);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Serialize<MsgPersistence>((MsgPersistence)res);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Serialize<MsgGetReloadConfigOptions>((MsgGetReloadConfigOptions)res);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Serialize<MsgGetConnectedInfos>((MsgGetConnectedInfos)res);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Serialize<MsgViewMongoDumpList>((MsgViewMongoDumpList)res);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Serialize<A_ResGetServiceConfigs>((A_ResGetServiceConfigs)res);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Serialize<MsgGetServiceConfigs>((MsgGetServiceConfigs)res);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Serialize<MsgGatewayServiceAction>((MsgGatewayServiceAction)res);

                case MsgType._Gateway_DestroyUser:
                    return MessagePackSerializer.Serialize<MsgGatewayDestroyUser>((MsgGatewayDestroyUser)res);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Serialize<MsgGatewayServerKick>((MsgGatewayServerKick)res);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Serialize<MsgUserServiceAction>((MsgUserServiceAction)res);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Serialize<MsgUserLoginSuccess>((MsgUserLoginSuccess)res);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Serialize<MsgUserServerKick>((MsgUserServerKick)res);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Serialize<MsgUserDisconnectFromGateway>((MsgUserDisconnectFromGateway)res);

                case MsgType._User_DestroyUser:
                    return MessagePackSerializer.Serialize<MsgUserDestroyUser>((MsgUserDestroyUser)res);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Serialize<MsgSaveUser>((MsgSaveUser)res);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Serialize<MsgGetUserCount>((MsgGetUserCount)res);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Serialize<MsgSaveUserInfoToFile>((MsgSaveUserInfoToFile)res);

                case MsgType._User_SaveUser:
                    return MessagePackSerializer.Serialize<MsgSaveUser>((MsgSaveUser)res);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Serialize<MsgSetGmFlag>((MsgSetGmFlag)res);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Serialize<MsgUserManagerUserLogin>((MsgUserManagerUserLogin)res);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Serialize<MsgRoomServiceAction>((MsgRoomServiceAction)res);

                case MsgType._Room_DestroyRoom:
                    return MessagePackSerializer.Serialize<MsgRoomDestroyRoom>((MsgRoomDestroyRoom)res);

                case MsgType._Room_SaveRoom:
                    return MessagePackSerializer.Serialize<MsgSaveRoom>((MsgSaveRoom)res);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Serialize<MsgSaveRoomInfoToFile>((MsgSaveRoomInfoToFile)res);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Serialize<MsgRoomUserEnter>((MsgRoomUserEnter)res);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Serialize<MsgRoomUserLeave>((MsgRoomUserLeave)res);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Serialize<MsgRoomLoadRoom>((MsgRoomLoadRoom)res);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Serialize<MsgSaveRoom>((MsgSaveRoom)res);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Serialize<MsgRoomManagerLoadRoom>((MsgRoomManagerLoadRoom)res);

                case MsgType._Command_PerformReloadScript:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformSaveUserInfoToFile:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformShowScriptVersion:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformGetPendingMsgList:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformShutdown:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformPlayerGM:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformKick:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Command_PerformSetPlayerGmFlag:
                    return MessagePackSerializer.Serialize<MsgCommon>((MsgCommon)res);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Serialize<MsgSave_AccountInfo>((MsgSave_AccountInfo)res);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize<MsgQuery_AccountInfo_byElementOf_userIds>((MsgQuery_AccountInfo_byElementOf_userIds)res);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Serialize<MsgQuery_AccountInfo_by_channelUserId>((MsgQuery_AccountInfo_by_channelUserId)res);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Serialize<MsgQuery_AccountInfo_by_channel_channelUserId>((MsgQuery_AccountInfo_by_channel_channelUserId)res);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize<MsgQuery_listOf_AccountInfo_byElementOf_userIds>((MsgQuery_listOf_AccountInfo_byElementOf_userIds)res);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Serialize<MsgQuery_UserInfo_by_userId>((MsgQuery_UserInfo_by_userId)res);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Serialize<MsgSave_UserInfo>((MsgSave_UserInfo)res);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Serialize<MsgInsert_UserInfo>((MsgInsert_UserInfo)res);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Serialize<MsgQuery_UserInfo_maxOf_userId>((MsgQuery_UserInfo_maxOf_userId)res);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Serialize<MsgQuery_RoomInfo_by_roomId>((MsgQuery_RoomInfo_by_roomId)res);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Serialize<MsgQuery_RoomInfo_maxOf_roomId>((MsgQuery_RoomInfo_maxOf_roomId)res);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Serialize<MsgInsert_RoomInfo>((MsgInsert_RoomInfo)res);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Serialize<MsgSave_RoomInfo>((MsgSave_RoomInfo)res);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Serialize<MsgLogin>((MsgLogin)res);

                case MsgType.Kick:
                    return MessagePackSerializer.Serialize<MsgKick>((MsgKick)res);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Serialize<MsgEnterRoom>((MsgEnterRoom)res);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Serialize<MsgLeaveRoom>((MsgLeaveRoom)res);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}