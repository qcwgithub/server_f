using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public static byte[] SerializeMsg(MsgType msgType, object msg)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Serialize((MsgShutdown)msg);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Serialize((MsgReloadScript)msg);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Serialize((MsgConnectorInfo)msg);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Serialize((MsgGetPendingMsgList)msg);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Serialize((MsgGetScriptVersion)msg);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Serialize((MsgReloadConfigs)msg);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Serialize((MsgGC)msg);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Serialize((MsgRemoteWillShutdown)msg);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Serialize((MsgGetServiceState)msg);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Serialize((MsgPersistence)msg);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Serialize((MsgGetReloadConfigOptions)msg);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Serialize((MsgGetConnectedInfos)msg);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Serialize((MsgViewMongoDumpList)msg);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Serialize((A_ResGetServiceConfigs)msg);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Serialize((MsgGetServiceConfigs)msg);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Serialize((MsgGatewayServiceAction)msg);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Serialize((MsgGatewayServerKick)msg);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Serialize((MsgUserServiceAction)msg);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Serialize((MsgUserLoginSuccess)msg);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Serialize((MsgUserServerKick)msg);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Serialize((MsgUserDisconnectFromGateway)msg);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Serialize((MsgSaveUserImmediately)msg);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Serialize((MsgGetUserCount)msg);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Serialize((MsgSaveUserInfoToFile)msg);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Serialize((MsgSetGmFlag)msg);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Serialize((MsgUserManagerUserLogin)msg);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Serialize((MsgRoomServiceAction)msg);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Serialize((MsgSaveRoomInfoToFile)msg);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Serialize((MsgRoomUserEnter)msg);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Serialize((MsgRoomUserLeave)msg);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Serialize((MsgRoomLoadRoom)msg);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Serialize((MsgSaveRoomImmediately)msg);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Serialize((MsgRoomManagerLoadRoom)msg);

                case MsgType._Command_PerformReloadScript:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformSaveUserInfoToFile:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformShowScriptVersion:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformGetPendingMsgList:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformShutdown:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformPlayerGM:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformKick:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Command_PerformSetPlayerGmFlag:
                    return MessagePackSerializer.Serialize((MsgCommon)msg);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Serialize((MsgSave_AccountInfo)msg);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize((MsgQuery_AccountInfo_byElementOf_userIds)msg);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Serialize((MsgQuery_AccountInfo_by_channelUserId)msg);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Serialize((MsgQuery_AccountInfo_by_channel_channelUserId)msg);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize((MsgQuery_listOf_AccountInfo_byElementOf_userIds)msg);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Serialize((MsgQuery_UserInfo_by_userId)msg);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Serialize((MsgSave_UserInfo)msg);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Serialize((MsgInsert_UserInfo)msg);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Serialize((MsgQuery_UserInfo_maxOf_userId)msg);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Serialize((MsgQuery_RoomInfo_by_roomId)msg);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Serialize((MsgQuery_RoomInfo_maxOf_roomId)msg);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Serialize((MsgInsert_RoomInfo)msg);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Serialize((MsgSave_RoomInfo)msg);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Serialize((MsgLogin)msg);

                case MsgType.Kick:
                    return MessagePackSerializer.Serialize((MsgKick)msg);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Serialize((MsgEnterRoom)msg);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Serialize((MsgLeaveRoom)msg);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}