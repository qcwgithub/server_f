using MessagePack;

namespace Data
{
    public partial class MessageTypeConfigData
    {
        public static object DeserializeRes(MsgType msgType, byte[] resBytes)
        {
            if (resBytes == null || resBytes.Length == 0)
            {
                return null;
            }

            object ob;
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Timer:
                    ob = MessagePackSerializer.Deserialize<ResTimer>(resBytes);
                    break;

                case MsgType._Service_Shutdown:
                    ob = MessagePackSerializer.Deserialize<ResShutdown>(resBytes);
                    break;

                case MsgType._Service_ReloadScript:
                    ob = MessagePackSerializer.Deserialize<ResReloadScript>(resBytes);
                    break;

                case MsgType._Service_ConnectorInfo:
                    ob = MessagePackSerializer.Deserialize<ResConnectorInfo>(resBytes);
                    break;

                case MsgType._Service_GetPendingMessageList:
                    ob = MessagePackSerializer.Deserialize<ResGetPendingMsgList>(resBytes);
                    break;

                case MsgType._Service_GetScriptVersion:
                    ob = MessagePackSerializer.Deserialize<ResGetScriptVersion>(resBytes);
                    break;

                case MsgType._Service_ReloadConfigs:
                    ob = MessagePackSerializer.Deserialize<ResReloadConfigs>(resBytes);
                    break;

                case MsgType._Service_GC:
                    ob = MessagePackSerializer.Deserialize<ResGC>(resBytes);
                    break;

                case MsgType._Service_RemoteWillShutdown:
                    ob = MessagePackSerializer.Deserialize<ResRemoteWillShutdown>(resBytes);
                    break;

                case MsgType._Service_GetServiceState:
                    ob = MessagePackSerializer.Deserialize<ResGetServiceState>(resBytes);
                    break;

                case MsgType._Service_GetReloadConfigOptions:
                    ob = MessagePackSerializer.Deserialize<ResGetReloadConfigOptions>(resBytes);
                    break;

                case MsgType._Service_GetConnectedInfos:
                    ob = MessagePackSerializer.Deserialize<ResGetConnectedInfos>(resBytes);
                    break;

                case MsgType._Service_ViewMongoDumpList:
                    ob = MessagePackSerializer.Deserialize<ResViewMongoDumpList>(resBytes);
                    break;

                case MsgType._Service_A_ResGetServiceConfigs:
                    ob = MessagePackSerializer.Deserialize<ResNull>(resBytes);
                    break;

                case MsgType._Global_GetServiceConfigs:
                    ob = MessagePackSerializer.Deserialize<ResGetServiceConfigs>(resBytes);
                    break;

                case MsgType._Gateway_ServerAction:
                    ob = MessagePackSerializer.Deserialize<ResGatewayServiceAction>(resBytes);
                    break;

                case MsgType._Gateway_ServerKick:
                    ob = MessagePackSerializer.Deserialize<ResGatewayServerKick>(resBytes);
                    break;

                case MsgType._User_ServerAction:
                    ob = MessagePackSerializer.Deserialize<ResUserServiceAction>(resBytes);
                    break;

                case MsgType._User_UserLoginSuccess:
                    ob = MessagePackSerializer.Deserialize<ResUserLoginSuccess>(resBytes);
                    break;

                case MsgType._User_ServerKick:
                    ob = MessagePackSerializer.Deserialize<ResUserServerKick>(resBytes);
                    break;

                case MsgType._User_UserDisconnectFromGateway:
                    ob = MessagePackSerializer.Deserialize<ResUserDisconnectFromGateway>(resBytes);
                    break;

                case MsgType._User_SaveUserImmediately:
                    ob = MessagePackSerializer.Deserialize<ResSaveUserImmediately>(resBytes);
                    break;

                case MsgType._User_GetUserCount:
                    ob = MessagePackSerializer.Deserialize<ResGetUserCount>(resBytes);
                    break;

                case MsgType._User_SaveUserInfoToFile:
                    ob = MessagePackSerializer.Deserialize<ResSaveUserInfoToFile>(resBytes);
                    break;

                case MsgType._User_SetGmFlag:
                    ob = MessagePackSerializer.Deserialize<ResSetGmFlag>(resBytes);
                    break;

                case MsgType._UserManager_UserLogin:
                    ob = MessagePackSerializer.Deserialize<ResUserManagerUserLogin>(resBytes);
                    break;

                case MsgType._Room_ServerAction:
                    ob = MessagePackSerializer.Deserialize<ResRoomServiceAction>(resBytes);
                    break;

                case MsgType._Room_SaveRoomInfoToFile:
                    ob = MessagePackSerializer.Deserialize<ResSaveRoomInfoToFile>(resBytes);
                    break;

                case MsgType._Room_UserEnter:
                    ob = MessagePackSerializer.Deserialize<ResRoomUserEnter>(resBytes);
                    break;

                case MsgType._Room_UserLeave:
                    ob = MessagePackSerializer.Deserialize<ResRoomUserLeave>(resBytes);
                    break;

                case MsgType._Room_LoadRoom:
                    ob = MessagePackSerializer.Deserialize<ResRoomLoadRoom>(resBytes);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    ob = MessagePackSerializer.Deserialize<ResSaveRoomImmediately>(resBytes);
                    break;

                case MsgType._Room_SendChat:
                    ob = MessagePackSerializer.Deserialize<ResRoomSendChat>(resBytes);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    ob = MessagePackSerializer.Deserialize<ResRoomManagerLoadRoom>(resBytes);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    ob = MessagePackSerializer.Deserialize<ResRoomManagerImportRoomConfig>(resBytes);
                    break;

                case MsgType._Save_AccountInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_AccountInfo>(resBytes);
                    break;

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    ob = MessagePackSerializer.Deserialize<ResQuery_AccountInfo_byElementOf_userIds>(resBytes);
                    break;

                case MsgType._Query_AccountInfo_by_channelUserId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channelUserId>(resBytes);
                    break;

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_AccountInfo_by_channel_channelUserId>(resBytes);
                    break;

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    ob = MessagePackSerializer.Deserialize<ResQuery_listOf_AccountInfo_byElementOf_userIds>(resBytes);
                    break;

                case MsgType._Query_UserInfo_by_userId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_UserInfo_by_userId>(resBytes);
                    break;

                case MsgType._Save_UserInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_UserInfo>(resBytes);
                    break;

                case MsgType._Insert_UserInfo:
                    ob = MessagePackSerializer.Deserialize<ResInsert_UserInfo>(resBytes);
                    break;

                case MsgType._Query_UserInfo_maxOf_userId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_UserInfo_maxOf_userId>(resBytes);
                    break;

                case MsgType._Query_RoomInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_RoomInfo_by_roomId>(resBytes);
                    break;

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_RoomInfo_maxOf_roomId>(resBytes);
                    break;

                case MsgType._Insert_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<ResInsert_RoomInfo>(resBytes);
                    break;

                case MsgType._Save_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_RoomInfo>(resBytes);
                    break;

                case MsgType._Search_RoomInfo:
                    ob = MessagePackSerializer.Deserialize<ResSearch_RoomInfo>(resBytes);
                    break;

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Forward:
                    throw new Exception("Missing config for MsgType.Forward");

                case MsgType.Login:
                    ob = MessagePackSerializer.Deserialize<ResLogin>(resBytes);
                    break;

                case MsgType.Kick:
                    ob = MessagePackSerializer.Deserialize<ResKick>(resBytes);
                    break;

                case MsgType.EnterRoom:
                    ob = MessagePackSerializer.Deserialize<ResEnterRoom>(resBytes);
                    break;

                case MsgType.LeaveRoom:
                    ob = MessagePackSerializer.Deserialize<ResLeaveRoom>(resBytes);
                    break;

                case MsgType.SendRoomChat:
                    ob = MessagePackSerializer.Deserialize<ResSendRoomChat>(resBytes);
                    break;

                case MsgType.A_RoomChat:
                    throw new Exception("Missing config for MsgType.A_RoomChat");

                case MsgType.SearchRoom:
                    ob = MessagePackSerializer.Deserialize<ResSearchRoom>(resBytes);
                    break;

                case MsgType.GetRecommendedRooms:
                    ob = MessagePackSerializer.Deserialize<ResGetRecommendedRooms>(resBytes);
                    break;

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }

            return ob;
        }
    }
}