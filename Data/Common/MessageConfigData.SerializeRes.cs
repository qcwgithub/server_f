using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public static byte[] SerializeRes(MsgType msgType, object res)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Timer:
                    return MessagePackSerializer.Serialize((ResTimer)res);

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Serialize((ResShutdown)res);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Serialize((ResReloadScript)res);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Serialize((ResConnectorInfo)res);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Serialize((ResGetPendingMsgList)res);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Serialize((ResGetScriptVersion)res);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Serialize((ResReloadConfigs)res);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Serialize((ResGC)res);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Serialize((ResRemoteWillShutdown)res);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Serialize((ResGetServiceState)res);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Serialize((ResPersistence)res);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Serialize((ResGetReloadConfigOptions)res);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Serialize((ResGetConnectedInfos)res);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Serialize((ResViewMongoDumpList)res);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Serialize((ResNull)res);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Serialize((ResGetServiceConfigs)res);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Serialize((ResGatewayServiceAction)res);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Serialize((ResGatewayServerKick)res);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Serialize((ResUserServiceAction)res);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Serialize((ResUserLoginSuccess)res);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Serialize((ResUserServerKick)res);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Serialize((ResUserDisconnectFromGateway)res);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Serialize((ResSaveUserImmediately)res);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Serialize((ResGetUserCount)res);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Serialize((ResSaveUserInfoToFile)res);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Serialize((ResSetGmFlag)res);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Serialize((ResUserManagerUserLogin)res);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Serialize((ResRoomServiceAction)res);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Serialize((ResSaveRoomInfoToFile)res);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Serialize((ResRoomUserEnter)res);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Serialize((ResRoomUserLeave)res);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Serialize((ResRoomLoadRoom)res);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Serialize((ResSaveRoomImmediately)res);

                case MsgType._Room_SendChat:
                    return MessagePackSerializer.Serialize((ResRoomSendChat)res);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Serialize((ResRoomManagerLoadRoom)res);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Serialize((ResSave_AccountInfo)res);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize((ResQuery_AccountInfo_byElementOf_userIds)res);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Serialize((ResQuery_AccountInfo_by_channelUserId)res);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Serialize((ResQuery_AccountInfo_by_channel_channelUserId)res);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Serialize((ResQuery_listOf_AccountInfo_byElementOf_userIds)res);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Serialize((ResQuery_UserInfo_by_userId)res);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Serialize((ResSave_UserInfo)res);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Serialize((ResInsert_UserInfo)res);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Serialize((ResQuery_UserInfo_maxOf_userId)res);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Serialize((ResQuery_RoomInfo_by_roomId)res);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Serialize((ResQuery_RoomInfo_maxOf_roomId)res);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Serialize((ResInsert_RoomInfo)res);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Serialize((ResSave_RoomInfo)res);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Serialize((ResLogin)res);

                case MsgType.Kick:
                    return MessagePackSerializer.Serialize((ResKick)res);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Serialize((ResEnterRoom)res);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Serialize((ResLeaveRoom)res);

                case MsgType.SendRoomChat:
                    return MessagePackSerializer.Serialize((ResSendRoomChat)res);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}