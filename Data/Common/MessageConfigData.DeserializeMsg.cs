using MessagePack;

namespace Data
{
    public partial class MessageConfigData
    {
        public static object DeserializeMsg(MsgType msgType, ArraySegment<byte> msgBytes)
        {
            switch (msgType)
            {
                #region auto

                case MsgType._Service_ReloadScript:
                    return MessagePackSerializer.Deserialize<MsgReloadScript>(msgBytes);

                case MsgType._Service_CheckConnections_Loop:
                    return MessagePackSerializer.Deserialize<MsgCheckConnections_Loop>(msgBytes);

                case MsgType._Service_ConnectorInfo:
                    return MessagePackSerializer.Deserialize<MsgConnectorInfo>(msgBytes);

                case MsgType._Service_GetPendingMessageList:
                    return MessagePackSerializer.Deserialize<MsgGetPendingMsgList>(msgBytes);

                case MsgType._Service_GetScriptVersion:
                    return MessagePackSerializer.Deserialize<MsgGetScriptVersion>(msgBytes);

                case MsgType._Service_ReloadConfigs:
                    return MessagePackSerializer.Deserialize<MsgReloadConfigs>(msgBytes);

                case MsgType._Service_GC:
                    return MessagePackSerializer.Deserialize<MsgGC>(msgBytes);

                case MsgType._Service_RemoteWillShutdown:
                    return MessagePackSerializer.Deserialize<MsgRemoteWillShutdown>(msgBytes);

                case MsgType._Service_GetServiceState:
                    return MessagePackSerializer.Deserialize<MsgGetServiceState>(msgBytes);

                case MsgType._Service_PersistenceTaskQueueHandler:
                    return MessagePackSerializer.Deserialize<MsgPersistence>(msgBytes);

                case MsgType._Service_PersistenceTaskQueueHandler_Loop:
                    throw new Exception("Missing config for MsgType._Service_PersistenceTaskQueueHandler_Loop");

                case MsgType._Service_GetReloadConfigOptions:
                    return MessagePackSerializer.Deserialize<MsgGetReloadConfigOptions>(msgBytes);

                case MsgType._Service_GetConnectedInfos:
                    return MessagePackSerializer.Deserialize<MsgGetConnectedInfos>(msgBytes);

                case MsgType._Service_ViewMongoDumpList:
                    return MessagePackSerializer.Deserialize<MsgViewMongoDumpList>(msgBytes);

                case MsgType._Service_A_ResGetServiceConfigs:
                    return MessagePackSerializer.Deserialize<A_ResGetServiceConfigs>(msgBytes);

                case MsgType._Global_GetServiceConfigs:
                    return MessagePackSerializer.Deserialize<MsgGetServiceConfigs>(msgBytes);

                case MsgType._Gateway_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgGatewayServiceAction>(msgBytes);

                case MsgType._Gateway_ServerKick:
                    return MessagePackSerializer.Deserialize<MsgGatewayServerKick>(msgBytes);

                case MsgType._User_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgUserServiceAction>(msgBytes);

                case MsgType._User_UserLoginSuccess:
                    return MessagePackSerializer.Deserialize<MsgUserLoginSuccess>(msgBytes);

                case MsgType._User_ServerKick:
                    return MessagePackSerializer.Deserialize<MsgUserServerKick>(msgBytes);

                case MsgType._User_UserDisconnectFromGateway:
                    return MessagePackSerializer.Deserialize<MsgUserDisconnectFromGateway>(msgBytes);

                case MsgType._User_SaveUserImmediately:
                    return MessagePackSerializer.Deserialize<MsgSaveUserImmediately>(msgBytes);

                case MsgType._User_GetUserCount:
                    return MessagePackSerializer.Deserialize<MsgGetUserCount>(msgBytes);

                case MsgType._User_SaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgSaveUserInfoToFile>(msgBytes);

                case MsgType._User_SetGmFlag:
                    return MessagePackSerializer.Deserialize<MsgSetGmFlag>(msgBytes);

                case MsgType._UserManager_UserLogin:
                    return MessagePackSerializer.Deserialize<MsgUserManagerUserLogin>(msgBytes);

                case MsgType._Room_ServerAction:
                    return MessagePackSerializer.Deserialize<MsgRoomServiceAction>(msgBytes);

                case MsgType._Room_SaveRoomInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgSaveRoomInfoToFile>(msgBytes);

                case MsgType._Room_UserEnter:
                    return MessagePackSerializer.Deserialize<MsgRoomUserEnter>(msgBytes);

                case MsgType._Room_UserLeave:
                    return MessagePackSerializer.Deserialize<MsgRoomUserLeave>(msgBytes);

                case MsgType._Room_LoadRoom:
                    return MessagePackSerializer.Deserialize<MsgRoomLoadRoom>(msgBytes);

                case MsgType._Room_SaveRoomImmediately:
                    return MessagePackSerializer.Deserialize<MsgSaveRoomImmediately>(msgBytes);

                case MsgType._RoomManager_LoadRoom:
                    return MessagePackSerializer.Deserialize<MsgRoomManagerLoadRoom>(msgBytes);

                case MsgType._Command_PerformReloadScript:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformSaveUserInfoToFile:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformShowScriptVersion:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformGetPendingMsgList:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformShutdown:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformPlayerGM:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformKick:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Command_PerformSetPlayerGmFlag:
                    return MessagePackSerializer.Deserialize<MsgCommon>(msgBytes);

                case MsgType._Save_AccountInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_AccountInfo>(msgBytes);

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_byElementOf_userIds>(msgBytes);

                case MsgType._Query_AccountInfo_by_channelUserId:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channelUserId>(msgBytes);

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    return MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channel_channelUserId>(msgBytes);

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    return MessagePackSerializer.Deserialize<MsgQuery_listOf_AccountInfo_byElementOf_userIds>(msgBytes);

                case MsgType._Query_UserInfo_by_userId:
                    return MessagePackSerializer.Deserialize<MsgQuery_UserInfo_by_userId>(msgBytes);

                case MsgType._Save_UserInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_UserInfo>(msgBytes);

                case MsgType._Insert_UserInfo:
                    return MessagePackSerializer.Deserialize<MsgInsert_UserInfo>(msgBytes);

                case MsgType._Query_UserInfo_maxOf_userId:
                    return MessagePackSerializer.Deserialize<MsgQuery_UserInfo_maxOf_userId>(msgBytes);

                case MsgType._Query_RoomInfo_by_roomId:
                    return MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_by_roomId>(msgBytes);

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    return MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_maxOf_roomId>(msgBytes);

                case MsgType._Insert_RoomInfo:
                    return MessagePackSerializer.Deserialize<MsgInsert_RoomInfo>(msgBytes);

                case MsgType._Save_RoomInfo:
                    return MessagePackSerializer.Deserialize<MsgSave_RoomInfo>(msgBytes);

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Login:
                    return MessagePackSerializer.Deserialize<MsgLogin>(msgBytes);

                case MsgType.Kick:
                    return MessagePackSerializer.Deserialize<MsgKick>(msgBytes);

                case MsgType.EnterRoom:
                    return MessagePackSerializer.Deserialize<MsgEnterRoom>(msgBytes);

                case MsgType.LeaveRoom:
                    return MessagePackSerializer.Deserialize<MsgLeaveRoom>(msgBytes);

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
        }
    }
}