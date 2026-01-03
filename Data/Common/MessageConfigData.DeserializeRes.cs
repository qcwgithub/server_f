using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public static object DeserializeRes(MsgType msgType, ArraySegment<byte> resBytes)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Timer:
                    return MessagePackSerializer.Deserialize<ResTimer>(resBytes);

                case MsgType._Service_Shutdown:
                    return MessagePackSerializer.Deserialize<ResShutdown>(resBytes);

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Deserialize<ResReloadScript>(resBytes);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Deserialize<ResConnectorInfo>(resBytes);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Deserialize<ResGetPendingMsgList>(resBytes);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Deserialize<ResGetScriptVersion>(resBytes);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Deserialize<ResReloadConfigs>(resBytes);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Deserialize<ResGC>(resBytes);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Deserialize<ResRemoteWillShutdown>(resBytes);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Deserialize<ResGetServiceState>(resBytes);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Deserialize<ResPersistence>(resBytes);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Deserialize<ResGetReloadConfigOptions>(resBytes);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Deserialize<ResGetConnectedInfos>(resBytes);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Deserialize<ResViewMongoDumpList>(resBytes);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Deserialize<ResNull>(resBytes);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Deserialize<ResGetServiceConfigs>(resBytes);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Deserialize<ResGatewayServiceAction>(resBytes);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Deserialize<ResGatewayServerKick>(resBytes);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Deserialize<ResUserServiceAction>(resBytes);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Deserialize<ResUserLoginSuccess>(resBytes);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Deserialize<ResUserServerKick>(resBytes);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Deserialize<ResUserDisconnectFromGateway>(resBytes);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Deserialize<ResSaveUserImmediately>(resBytes);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Deserialize<ResGetUserCount>(resBytes);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<ResSaveUserInfoToFile>(resBytes);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Deserialize<ResSetGmFlag>(resBytes);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Deserialize<ResUserManagerUserLogin>(resBytes);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Deserialize<ResRoomServiceAction>(resBytes);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Deserialize<ResSaveRoomInfoToFile>(resBytes);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Deserialize<ResRoomUserEnter>(resBytes);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Deserialize<ResRoomUserLeave>(resBytes);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Deserialize<ResRoomLoadRoom>(resBytes);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Deserialize<ResSaveRoomImmediately>(resBytes);

                case MsgType._Room_SendChat:
                    return MessagePackSerializer.Deserialize<ResRoomSendChat>(resBytes);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Deserialize<ResRoomManagerLoadRoom>(resBytes);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Deserialize<ResSave_AccountInfo>(resBytes);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_byElementOf_userIds>(resBytes);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channelUserId>(resBytes);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channel_channelUserId>(resBytes);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<ResQuery_listOf_AccountInfo_byElementOf_userIds>(resBytes);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Deserialize<ResQuery_UserInfo_by_userId>(resBytes);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Deserialize<ResSave_UserInfo>(resBytes);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Deserialize<ResInsert_UserInfo>(resBytes);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Deserialize<ResQuery_UserInfo_maxOf_userId>(resBytes);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Deserialize<ResQuery_RoomInfo_by_roomId>(resBytes);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Deserialize<ResQuery_RoomInfo_maxOf_roomId>(resBytes);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Deserialize<ResInsert_RoomInfo>(resBytes);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Deserialize<ResSave_RoomInfo>(resBytes);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Deserialize<ResLogin>(resBytes);

                case MsgType.Kick:
                    return MessagePackSerializer.Deserialize<ResKick>(resBytes);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Deserialize<ResEnterRoom>(resBytes);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Deserialize<ResLeaveRoom>(resBytes);

                case MsgType.SendRoomChat:
                    return MessagePackSerializer.Deserialize<ResSendRoomChat>(resBytes);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}