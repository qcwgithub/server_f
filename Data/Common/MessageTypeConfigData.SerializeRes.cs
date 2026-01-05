using MessagePack;

namespace Data
{
    public partial class MessageTypeConfigData
    {
        public static byte[] SerializeRes(MsgType msgType, object res)
        {
            byte[] msgBytes;
            switch (msgType)
            {
                #region auto

                case MsgType._Service_Timer:
                    msgBytes = MessagePackSerializer.Serialize((ResTimer)res);
                    break;

                case MsgType._Service_Shutdown:
                    msgBytes = MessagePackSerializer.Serialize((ResShutdown)res);
                    break;

                case MsgType._Service_ReloadScript:
                    msgBytes = MessagePackSerializer.Serialize((ResReloadScript)res);
                    break;

                case MsgType._Service_ConnectorInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResConnectorInfo)res);
                    break;

                case MsgType._Service_GetPendingMessageList:
                    msgBytes = MessagePackSerializer.Serialize((ResGetPendingMsgList)res);
                    break;

                case MsgType._Service_GetScriptVersion:
                    msgBytes = MessagePackSerializer.Serialize((ResGetScriptVersion)res);
                    break;

                case MsgType._Service_ReloadConfigs:
                    msgBytes = MessagePackSerializer.Serialize((ResReloadConfigs)res);
                    break;

                case MsgType._Service_GC:
                    msgBytes = MessagePackSerializer.Serialize((ResGC)res);
                    break;

                case MsgType._Service_RemoteWillShutdown:
                    msgBytes = MessagePackSerializer.Serialize((ResRemoteWillShutdown)res);
                    break;

                case MsgType._Service_GetServiceState:
                    msgBytes = MessagePackSerializer.Serialize((ResGetServiceState)res);
                    break;

                case MsgType._Service_GetReloadConfigOptions:
                    msgBytes = MessagePackSerializer.Serialize((ResGetReloadConfigOptions)res);
                    break;

                case MsgType._Service_GetConnectedInfos:
                    msgBytes = MessagePackSerializer.Serialize((ResGetConnectedInfos)res);
                    break;

                case MsgType._Service_ViewMongoDumpList:
                    msgBytes = MessagePackSerializer.Serialize((ResViewMongoDumpList)res);
                    break;

                case MsgType._Service_A_ResGetServiceConfigs:
                    msgBytes = MessagePackSerializer.Serialize((ResNull)res);
                    break;

                case MsgType._Global_GetServiceConfigs:
                    msgBytes = MessagePackSerializer.Serialize((ResGetServiceConfigs)res);
                    break;

                case MsgType._Gateway_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((ResGatewayServiceAction)res);
                    break;

                case MsgType._Gateway_ServerKick:
                    msgBytes = MessagePackSerializer.Serialize((ResGatewayServerKick)res);
                    break;

                case MsgType._User_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((ResUserServiceAction)res);
                    break;

                case MsgType._User_UserLoginSuccess:
                    msgBytes = MessagePackSerializer.Serialize((ResUserLoginSuccess)res);
                    break;

                case MsgType._User_ServerKick:
                    msgBytes = MessagePackSerializer.Serialize((ResUserServerKick)res);
                    break;

                case MsgType._User_UserDisconnectFromGateway:
                    msgBytes = MessagePackSerializer.Serialize((ResUserDisconnectFromGateway)res);
                    break;

                case MsgType._User_SaveUserImmediately:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveUserImmediately)res);
                    break;

                case MsgType._User_GetUserCount:
                    msgBytes = MessagePackSerializer.Serialize((ResGetUserCount)res);
                    break;

                case MsgType._User_SaveUserInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveUserInfoToFile)res);
                    break;

                case MsgType._User_SetGmFlag:
                    msgBytes = MessagePackSerializer.Serialize((ResSetGmFlag)res);
                    break;

                case MsgType._UserManager_UserLogin:
                    msgBytes = MessagePackSerializer.Serialize((ResUserManagerUserLogin)res);
                    break;

                case MsgType._Room_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomServiceAction)res);
                    break;

                case MsgType._Room_SaveRoomInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveRoomInfoToFile)res);
                    break;

                case MsgType._Room_UserEnter:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomUserEnter)res);
                    break;

                case MsgType._Room_UserLeave:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomUserLeave)res);
                    break;

                case MsgType._Room_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomLoadRoom)res);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveRoomImmediately)res);
                    break;

                case MsgType._Room_SendChat:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomSendChat)res);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerLoadRoom)res);
                    break;

                case MsgType._Save_AccountInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_AccountInfo)res);
                    break;

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_AccountInfo_byElementOf_userIds)res);
                    break;

                case MsgType._Query_AccountInfo_by_channelUserId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_AccountInfo_by_channelUserId)res);
                    break;

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_AccountInfo_by_channel_channelUserId)res);
                    break;

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_listOf_AccountInfo_byElementOf_userIds)res);
                    break;

                case MsgType._Query_UserInfo_by_userId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_UserInfo_by_userId)res);
                    break;

                case MsgType._Save_UserInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_UserInfo)res);
                    break;

                case MsgType._Insert_UserInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_UserInfo)res);
                    break;

                case MsgType._Query_UserInfo_maxOf_userId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_UserInfo_maxOf_userId)res);
                    break;

                case MsgType._Query_RoomInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_RoomInfo_by_roomId)res);
                    break;

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_RoomInfo_maxOf_roomId)res);
                    break;

                case MsgType._Insert_RoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_RoomInfo)res);
                    break;

                case MsgType._Save_RoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_RoomInfo)res);
                    break;

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Forward:
                    throw new Exception("Missing config for MsgType.Forward");

                case MsgType.Login:
                    msgBytes = MessagePackSerializer.Serialize((ResLogin)res);
                    break;

                case MsgType.Kick:
                    msgBytes = MessagePackSerializer.Serialize((ResKick)res);
                    break;

                case MsgType.EnterRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResEnterRoom)res);
                    break;

                case MsgType.LeaveRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResLeaveRoom)res);
                    break;

                case MsgType.SendRoomChat:
                    msgBytes = MessagePackSerializer.Serialize((ResSendRoomChat)res);
                    break;

                case MsgType.A_RoomChat:
                    throw new Exception("Missing config for MsgType.A_RoomChat");

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }
            return msgBytes;
        }
    }
}