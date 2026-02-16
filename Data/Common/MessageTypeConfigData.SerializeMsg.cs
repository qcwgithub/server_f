using MessagePack;

namespace Data
{
    public partial class MessageTypeConfigData
    {
        public static byte[] SerializeMsg(MsgType msgType, object msg)
        {
            if (msg == null)
            {
                return [];
            }

            byte[] msgBytes;
            switch (msgType)
            {
                #region auto

                case MsgType._Timer:
                    msgBytes = MessagePackSerializer.Serialize((MsgTimer)msg);
                    break;

                case MsgType._Shutdown:
                    msgBytes = MessagePackSerializer.Serialize((MsgShutdown)msg);
                    break;

                case MsgType._ReloadScript:
                    msgBytes = MessagePackSerializer.Serialize((MsgReloadScript)msg);
                    break;

                case MsgType._ConnectorInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgConnectorInfo)msg);
                    break;

                case MsgType._GetPendingMessageList:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetPendingMsgList)msg);
                    break;

                case MsgType._GetScriptVersion:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetScriptVersion)msg);
                    break;

                case MsgType._ReloadConfigs:
                    msgBytes = MessagePackSerializer.Serialize((MsgReloadConfigs)msg);
                    break;

                case MsgType._GC:
                    msgBytes = MessagePackSerializer.Serialize((MsgGC)msg);
                    break;

                case MsgType._RemoteWillShutdown:
                    msgBytes = MessagePackSerializer.Serialize((MsgRemoteWillShutdown)msg);
                    break;

                case MsgType._GetServiceState:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetServiceState)msg);
                    break;

                case MsgType._GetReloadConfigOptions:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetReloadConfigOptions)msg);
                    break;

                case MsgType._GetConnectedInfos:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetConnectedInfos)msg);
                    break;

                case MsgType._ViewMongoDumpList:
                    msgBytes = MessagePackSerializer.Serialize((MsgViewMongoDumpList)msg);
                    break;

                case MsgType._A_ResGetServiceConfigs:
                    msgBytes = MessagePackSerializer.Serialize((A_ResGetServiceConfigs)msg);
                    break;

                case MsgType._Global_GetServiceConfigs:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetServiceConfigs)msg);
                    break;

                case MsgType._Gateway_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((MsgGatewayServiceAction)msg);
                    break;

                case MsgType._Gateway_ServerKick:
                    msgBytes = MessagePackSerializer.Serialize((MsgGatewayServerKick)msg);
                    break;

                case MsgType._User_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserServiceAction)msg);
                    break;

                case MsgType._User_UserLoginSuccess:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserLoginSuccess)msg);
                    break;

                case MsgType._User_ServerKick:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserServerKick)msg);
                    break;

                case MsgType._User_UserDisconnectFromGateway:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserDisconnectFromGateway)msg);
                    break;

                case MsgType._User_SaveUserImmediately:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveUserImmediately)msg);
                    break;

                case MsgType._User_GetUserCount:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetUserCount)msg);
                    break;

                case MsgType._User_SaveUserInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveUserInfoToFile)msg);
                    break;

                case MsgType._User_SetGmFlag:
                    msgBytes = MessagePackSerializer.Serialize((MsgSetGmFlag)msg);
                    break;

                case MsgType._User_ResetName:
                    msgBytes = MessagePackSerializer.Serialize((MsgResetName)msg);
                    break;

                case MsgType._User_ReceiveFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgReceiveFriendRequest)msg);
                    break;

                case MsgType._User_OtherAcceptFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgOtherAcceptFriendRequest)msg);
                    break;

                case MsgType._User_OtherRejectFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgOtherRejectFriendRequest)msg);
                    break;

                case MsgType._UserManager_UserLogin:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserManagerUserLogin)msg);
                    break;

                case MsgType._UserManager_GetUserLocation:
                    msgBytes = MessagePackSerializer.Serialize((MsgUserManagerGetUserLocation)msg);
                    break;

                case MsgType._UserManager_ForwardToUserService:
                    msgBytes = MessagePackSerializer.Serialize((MsgForwardToUserService)msg);
                    break;

                case MsgType._Room_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomServiceAction)msg);
                    break;

                case MsgType._Room_SaveRoomInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveRoomInfoToFile)msg);
                    break;

                case MsgType._Room_UserEnter:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomUserEnter)msg);
                    break;

                case MsgType._Room_UserLeave:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomUserLeave)msg);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveRoomImmediately)msg);
                    break;

                case MsgType._Room_SendChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomSendChat)msg);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomManagerLoadRoom)msg);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomManagerImportRoomConfig)msg);
                    break;

                case MsgType._Save_AccountInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_AccountInfo)msg);
                    break;

                case MsgType._Query_AccountInfo_byElementOf_userIds:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_AccountInfo_byElementOf_userIds)msg);
                    break;

                case MsgType._Query_AccountInfo_by_channelUserId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_AccountInfo_by_channelUserId)msg);
                    break;

                case MsgType._Query_AccountInfo_by_channel_channelUserId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_AccountInfo_by_channel_channelUserId)msg);
                    break;

                case MsgType._Query_listOf_AccountInfo_byElementOf_userIds:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_listOf_AccountInfo_byElementOf_userIds)msg);
                    break;

                case MsgType._Query_UserInfo_by_userId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_UserInfo_by_userId)msg);
                    break;

                case MsgType._Save_UserInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_UserInfo)msg);
                    break;

                case MsgType._Insert_UserInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgInsert_UserInfo)msg);
                    break;

                case MsgType._Query_UserInfo_maxOf_userId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_UserInfo_maxOf_userId)msg);
                    break;

                case MsgType._Query_RoomInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_RoomInfo_by_roomId)msg);
                    break;

                case MsgType._Query_RoomInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_RoomInfo_maxOf_roomId)msg);
                    break;

                case MsgType._Insert_RoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgInsert_RoomInfo)msg);
                    break;

                case MsgType._Save_RoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_RoomInfo)msg);
                    break;

                case MsgType._Search_RoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSearch_RoomInfo)msg);
                    break;

                case MsgType._Save_RoomMessageReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_RoomMessageReportInfo)msg);
                    break;

                case MsgType._Save_UserReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_UserReportInfo)msg);
                    break;

                case MsgType.ClientStart:
                    throw new Exception("Missing config for MsgType.ClientStart");

                case MsgType.Forward:
                    msgBytes = MessagePackSerializer.Serialize((MsgForward)msg);
                    break;

                case MsgType.Login:
                    msgBytes = MessagePackSerializer.Serialize((MsgLogin)msg);
                    break;

                case MsgType.Kick:
                    msgBytes = MessagePackSerializer.Serialize((MsgKick)msg);
                    break;

                case MsgType.EnterRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgEnterRoom)msg);
                    break;

                case MsgType.LeaveRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgLeaveRoom)msg);
                    break;

                case MsgType.SendRoomChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgSendRoomChat)msg);
                    break;

                case MsgType.ARoomChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgARoomChat)msg);
                    break;

                case MsgType.SearchRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgSearchRoom)msg);
                    break;

                case MsgType.GetRecommendedRooms:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetRecommendedRooms)msg);
                    break;

                case MsgType.SetName:
                    msgBytes = MessagePackSerializer.Serialize((MsgSetName)msg);
                    break;

                case MsgType.SetAvatarIndex:
                    msgBytes = MessagePackSerializer.Serialize((MsgSetAvatarIndex)msg);
                    break;

                case MsgType.GetRoomChatHistory:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetRoomChatHistory)msg);
                    break;

                case MsgType.ReportRoomMessage:
                    msgBytes = MessagePackSerializer.Serialize((MsgReportRoomMessage)msg);
                    break;

                case MsgType.ReportUser:
                    msgBytes = MessagePackSerializer.Serialize((MsgReportUser)msg);
                    break;

                case MsgType.SendFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgSendFriendRequest)msg);
                    break;

                case MsgType.RejectFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgRejectFriendRequest)msg);
                    break;

                case MsgType.AcceptFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgAcceptFriendRequest)msg);
                    break;

                case MsgType.BlockUser:
                    msgBytes = MessagePackSerializer.Serialize((MsgBlockUser)msg);
                    break;

                case MsgType.UnblockUser:
                    msgBytes = MessagePackSerializer.Serialize((MsgUnblockUser)msg);
                    break;

                case MsgType.Count:
                    throw new Exception("Missing config for MsgType.Count");

                #endregion auto

                default:
                    throw new Exception("Not handled MsgType." + msgType);
            }

            return msgBytes;
        }
    }
}