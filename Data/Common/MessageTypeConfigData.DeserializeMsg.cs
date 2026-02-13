using MessagePack;

namespace Data
{
    public partial class MessageTypeConfigData
    {
        public static object DeserializeMsg(MsgType msgType, byte[] msgBytes)
        {
            if (msgBytes == null || msgBytes.Length == 0)
            {
                return null;
            }

            object ob;
            switch (msgType)
            {
                #region auto

                case MsgType._Timer:
                    ob = MessagePackSerializer.Deserialize<MsgTimer>(msgBytes);
                    break;

                case MsgType._Shutdown:
                    ob = MessagePackSerializer.Deserialize<MsgShutdown>(msgBytes);
                    break;

                case MsgType._ReloadScript:
                    ob = MessagePackSerializer.Deserialize<MsgReloadScript>(msgBytes);
                    break;

                case MsgType._ConnectorInfo:
                    ob = MessagePackSerializer.Deserialize<MsgConnectorInfo>(msgBytes);
                    break;

                case MsgType._GetPendingMessageList:
                    ob = MessagePackSerializer.Deserialize<MsgGetPendingMsgList>(msgBytes);
                    break;

                case MsgType._GetScriptVersion:
                    ob = MessagePackSerializer.Deserialize<MsgGetScriptVersion>(msgBytes);
                    break;

                case MsgType._ReloadConfigs:
                    ob = MessagePackSerializer.Deserialize<MsgReloadConfigs>(msgBytes);
                    break;

                case MsgType._GC:
                    ob = MessagePackSerializer.Deserialize<MsgGC>(msgBytes);
                    break;

                case MsgType._RemoteWillShutdown:
                    ob = MessagePackSerializer.Deserialize<MsgRemoteWillShutdown>(msgBytes);
                    break;

                case MsgType._GetServiceState:
                    ob = MessagePackSerializer.Deserialize<MsgGetServiceState>(msgBytes);
                    break;

                case MsgType._GetReloadConfigOptions:
                    ob = MessagePackSerializer.Deserialize<MsgGetReloadConfigOptions>(msgBytes);
                    break;

                case MsgType._GetConnectedInfos:
                    ob = MessagePackSerializer.Deserialize<MsgGetConnectedInfos>(msgBytes);
                    break;

                case MsgType._ViewMongoDumpList:
                    ob = MessagePackSerializer.Deserialize<MsgViewMongoDumpList>(msgBytes);
                    break;

                case MsgType._A_ResGetServiceConfigs:
                    ob = MessagePackSerializer.Deserialize<A_ResGetServiceConfigs>(msgBytes);
                    break;

                case MsgType._Global_GetServiceConfigs:
                    ob = MessagePackSerializer.Deserialize<MsgGetServiceConfigs>(msgBytes);
                    break;

                case MsgType._Gateway_ServerAction:
                    ob = MessagePackSerializer.Deserialize<MsgGatewayServiceAction>(msgBytes);
                    break;

                case MsgType._Gateway_ServerKick:
                    ob = MessagePackSerializer.Deserialize<MsgGatewayServerKick>(msgBytes);
                    break;

                case MsgType._User_ServerAction:
                    ob = MessagePackSerializer.Deserialize<MsgUserServiceAction>(msgBytes);
                    break;

                case MsgType._User_UserLoginSuccess:
                    ob = MessagePackSerializer.Deserialize<MsgUserLoginSuccess>(msgBytes);
                    break;

                case MsgType._User_ServerKick:
                    ob = MessagePackSerializer.Deserialize<MsgUserServerKick>(msgBytes);
                    break;

                case MsgType._User_UserDisconnectFromGateway:
                    ob = MessagePackSerializer.Deserialize<MsgUserDisconnectFromGateway>(msgBytes);
                    break;

                case MsgType._User_SaveUserImmediately:
                    ob = MessagePackSerializer.Deserialize<MsgSaveUserImmediately>(msgBytes);
                    break;

                case MsgType._User_GetUserCount:
                    ob = MessagePackSerializer.Deserialize<MsgGetUserCount>(msgBytes);
                    break;

                case MsgType._User_SaveUserInfoToFile:
                    ob = MessagePackSerializer.Deserialize<MsgSaveUserInfoToFile>(msgBytes);
                    break;

                case MsgType._User_SetGmFlag:
                    ob = MessagePackSerializer.Deserialize<MsgSetGmFlag>(msgBytes);
                    break;

                case MsgType._User_ResetName:
                    ob = MessagePackSerializer.Deserialize<MsgResetName>(msgBytes);
                    break;

                case MsgType._UserManager_UserLogin:
                    ob = MessagePackSerializer.Deserialize<MsgUserManagerUserLogin>(msgBytes);
                    break;

                case MsgType._UserManager_GetUserLocation:
                    ob = MessagePackSerializer.Deserialize<MsgUserManagerGetUserLocation>(msgBytes);
                    break;

                case MsgType._Room_ServerAction:
                    ob = MessagePackSerializer.Deserialize<MsgRoomServiceAction>(msgBytes);
                    break;

                case MsgType._Room_SaveRoomInfoToFile:
                    ob = MessagePackSerializer.Deserialize<MsgSaveRoomInfoToFile>(msgBytes);
                    break;

                case MsgType._Room_UserEnter:
                    ob = MessagePackSerializer.Deserialize<MsgRoomUserEnter>(msgBytes);
                    break;

                case MsgType._Room_UserLeave:
                    ob = MessagePackSerializer.Deserialize<MsgRoomUserLeave>(msgBytes);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    ob = MessagePackSerializer.Deserialize<MsgSaveRoomImmediately>(msgBytes);
                    break;

                case MsgType._Room_SendChat:
                    ob = MessagePackSerializer.Deserialize<MsgRoomSendChat>(msgBytes);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    ob = MessagePackSerializer.Deserialize<MsgRoomManagerLoadRoom>(msgBytes);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    ob = MessagePackSerializer.Deserialize<MsgRoomManagerImportRoomConfig>(msgBytes);
                    break;

                case MsgType._Save_AccountInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_AccountInfo>(msgBytes);
                    break;

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_byElementOf_userIds>(msgBytes);
                    break;

                case MsgType._Query_AccountInfo_by_channelUserId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channelUserId>(msgBytes);
                    break;

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_AccountInfo_by_channel_channelUserId>(msgBytes);
                    break;

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_listOf_AccountInfo_byElementOf_userIds>(msgBytes);
                    break;

                case MsgType._Query_UserInfo_by_userId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_UserInfo_by_userId>(msgBytes);
                    break;

                case MsgType._Save_UserInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_UserInfo>(msgBytes);
                    break;

                case MsgType._Insert_UserInfo:
                    ob = MessagePackSerializer.Deserialize<MsgInsert_UserInfo>(msgBytes);
                    break;

                case MsgType._Query_UserInfo_maxOf_userId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_UserInfo_maxOf_userId>(msgBytes);
                    break;

                case MsgType._Query_RoomInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_by_roomId>(msgBytes);
                    break;

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_RoomInfo_maxOf_roomId>(msgBytes);
                    break;

                case MsgType._Insert_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<MsgInsert_RoomInfo>(msgBytes);
                    break;

                case MsgType._Save_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_RoomInfo>(msgBytes);
                    break;

                case MsgType._Search_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSearch_RoomInfo>(msgBytes);
                    break;

                case MsgType._Save_RoomMessageReportInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_RoomMessageReportInfo>(msgBytes);
                    break;

                case MsgType._Save_UserReportInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_UserReportInfo>(msgBytes);
                    break;

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Forward:
                    ob = MessagePackSerializer.Deserialize<MsgForward>(msgBytes);
                    break;

                case MsgType.Login:
                    ob = MessagePackSerializer.Deserialize<MsgLogin>(msgBytes);
                    break;

                case MsgType.Kick:
                    ob = MessagePackSerializer.Deserialize<MsgKick>(msgBytes);
                    break;

                case MsgType.EnterRoom:
                    ob = MessagePackSerializer.Deserialize<MsgEnterRoom>(msgBytes);
                    break;

                case MsgType.LeaveRoom:
                    ob = MessagePackSerializer.Deserialize<MsgLeaveRoom>(msgBytes);
                    break;

                case MsgType.SendRoomChat:
                    ob = MessagePackSerializer.Deserialize<MsgSendRoomChat>(msgBytes);
                    break;

                case MsgType.ARoomChat:
                    ob = MessagePackSerializer.Deserialize<MsgARoomChat>(msgBytes);
                    break;

                case MsgType.SearchRoom:
                    ob = MessagePackSerializer.Deserialize<MsgSearchRoom>(msgBytes);
                    break;

                case MsgType.GetRecommendedRooms:
                    ob = MessagePackSerializer.Deserialize<MsgGetRecommendedRooms>(msgBytes);
                    break;

                case MsgType.SetName:
                    ob = MessagePackSerializer.Deserialize<MsgSetName>(msgBytes);
                    break;

                case MsgType.SetAvatarIndex:
                    ob = MessagePackSerializer.Deserialize<MsgSetAvatarIndex>(msgBytes);
                    break;

                case MsgType.GetRoomChatHistory:
                    ob = MessagePackSerializer.Deserialize<MsgGetRoomChatHistory>(msgBytes);
                    break;

                case MsgType.ReportRoomMessage:
                    ob = MessagePackSerializer.Deserialize<MsgReportRoomMessage>(msgBytes);
                    break;

                case MsgType.ReportUser:
                    ob = MessagePackSerializer.Deserialize<MsgReportUser>(msgBytes);
                    break;

                case MsgType.Count:
                    throw new Exception("Missing config for MsgType.Count");

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }

            return ob;
        }
    }
}