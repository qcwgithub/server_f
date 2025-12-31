using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public static object DeserializeRes(MsgType msgType, ArraySegment<byte> res)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Start:
                    return MessagePackSerializer.Deserialize<ResStart>(res);

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Deserialize<ResShutdown>(res);

                case MsgType._Service_OnConnectComplete:
                    return MessagePackSerializer.Deserialize<ResOnConnectComplete>(res);

                case MsgType._Service_OnConnectionClose:
                    return MessagePackSerializer.Deserialize<ResOnConnectionClose>(res);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Deserialize<ResReloadScript>(res);

                case MsgType._Service_CheckConnections:
                    return MessagePackSerializer.Deserialize<ResCheckConnections>(res);

                case MsgType._Service_CheckConnections_Loop:
                    return MessagePackSerializer.Deserialize<ResCheckConnections_Loop>(res);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Deserialize<ResConnectorInfo>(res);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Deserialize<ResGetPendingMsgList>(res);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Deserialize<ResGetScriptVersion>(res);

                case MsgType._Service_OnHttpRequest:
                    return MessagePackSerializer.Deserialize<ResOnHttpRequest>(res);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Deserialize<ResReloadConfigs>(res);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Deserialize<ResGC>(res);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Deserialize<ResRemoteWillShutdown>(res);

                case MsgType._Service_WaitTask:
                    return MessagePackSerializer.Deserialize<ResWaitTask>(res);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Deserialize<ResGetServiceState>(res);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Deserialize<ResPersistence>(res);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Deserialize<ResGetReloadConfigOptions>(res);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Deserialize<ResGetConnectedInfos>(res);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Deserialize<ResViewMongoDumpList>(res);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Deserialize<ResNull>(res);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Deserialize<ResGetServiceConfigs>(res);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Deserialize<ResGatewayServiceAction>(res);

                case MsgType._Gateway_DestroyUser:
                    return MessagePackSerializer.Deserialize<ResGatewayDestroyUser>(res);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Deserialize<ResGatewayServerKick>(res);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Deserialize<ResUserServiceAction>(res);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Deserialize<ResUserLoginSuccess>(res);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Deserialize<ResUserServerKick>(res);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Deserialize<ResUserDisconnectFromGateway>(res);

                case MsgType._User_DestroyUser:
                    return MessagePackSerializer.Deserialize<ResUserDestroyUser>(res);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Deserialize<ResSaveUser>(res);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Deserialize<ResGetUserCount>(res);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<ResSaveUserInfoToFile>(res);

                case MsgType._User_SaveUser:
                    return MessagePackSerializer.Deserialize<ResSaveUser>(res);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Deserialize<ResSetGmFlag>(res);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Deserialize<ResUserManagerUserLogin>(res);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Deserialize<ResRoomServiceAction>(res);

                case MsgType._Room_DestroyRoom:
                    return MessagePackSerializer.Deserialize<ResRoomDestroyRoom>(res);

                case MsgType._Room_SaveRoom:
                    return MessagePackSerializer.Deserialize<ResSaveRoom>(res);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Deserialize<ResSaveRoomInfoToFile>(res);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Deserialize<ResRoomUserEnter>(res);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Deserialize<ResRoomUserLeave>(res);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Deserialize<ResRoomLoadRoom>(res);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Deserialize<ResSaveRoom>(res);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Deserialize<ResRoomManagerLoadRoom>(res);

                case MsgType._Command_PerformReloadScript:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformSaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformShowScriptVersion:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformGetPendingMsgList:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformShutdown:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformPlayerGM:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformKick:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Command_PerformSetPlayerGmFlag:
                    return MessagePackSerializer.Deserialize<ResCommon>(res);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Deserialize<ResSave_AccountInfo>(res);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_byElementOf_userIds>(res);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channelUserId>(res);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channel_channelUserId>(res);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<ResQuery_listOf_AccountInfo_byElementOf_userIds>(res);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Deserialize<ResQuery_UserInfo_by_userId>(res);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Deserialize<ResSave_UserInfo>(res);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Deserialize<ResInsert_UserInfo>(res);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Deserialize<ResQuery_UserInfo_maxOf_userId>(res);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Deserialize<ResQuery_RoomInfo_by_roomId>(res);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Deserialize<ResQuery_RoomInfo_maxOf_roomId>(res);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Deserialize<ResInsert_RoomInfo>(res);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Deserialize<ResSave_RoomInfo>(res);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Deserialize<ResLogin>(res);

                case MsgType.Kick:
                    return MessagePackSerializer.Deserialize<ResKick>(res);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Deserialize<ResEnterRoom>(res);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Deserialize<ResLeaveRoom>(res);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}