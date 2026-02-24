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

                case MsgType._User_OtherRemoveFriend:
                    msgBytes = MessagePackSerializer.Serialize((MsgOtherRemoveFriend)msg);
                    break;

                case MsgType._User_ReceiveChatMessage:
                    msgBytes = MessagePackSerializer.Serialize((MsgReceiveChatMessage)msg);
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

                case MsgType._Room_SaveSceneInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveSceneInfoToFile)msg);
                    break;

                case MsgType._Room_UserEnterScene:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomUserEnterScene)msg);
                    break;

                case MsgType._Room_UserLeaveScene:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomUserLeaveScene)msg);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    msgBytes = MessagePackSerializer.Serialize((MsgSaveRoomImmediately)msg);
                    break;

                case MsgType._Room_SendSceneChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomSendSceneChat)msg);
                    break;

                case MsgType._Room_SendFriendChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomSendFriendChat)msg);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomManagerLoadRoom)msg);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomManagerImportRoomConfig)msg);
                    break;

                case MsgType._RoomManager_CreateFriendChatRoom:
                    msgBytes = MessagePackSerializer.Serialize((MsgRoomManagerCreateFriendChatRoom)msg);
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

                case MsgType._Query_SceneInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_SceneInfo_by_roomId)msg);
                    break;

                case MsgType._Query_SceneInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_SceneInfo_maxOf_roomId)msg);
                    break;

                case MsgType._Insert_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgInsert_SceneInfo)msg);
                    break;

                case MsgType._Save_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_SceneInfo)msg);
                    break;

                case MsgType._Search_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSearch_SceneInfo)msg);
                    break;

                case MsgType._Save_MessageReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_MessageReportInfo)msg);
                    break;

                case MsgType._Save_UserReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_UserReportInfo)msg);
                    break;

                case MsgType._Save_UserBriefInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_UserBriefInfo)msg);
                    break;

                case MsgType._Query_UserBriefInfo_by_userId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_UserBriefInfo_by_userId)msg);
                    break;

                case MsgType._Query_FriendChatInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_FriendChatInfo_by_roomId)msg);
                    break;

                case MsgType._Query_FriendChatInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((MsgQuery_FriendChatInfo_maxOf_roomId)msg);
                    break;

                case MsgType._Insert_FriendChatInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgInsert_FriendChatInfo)msg);
                    break;

                case MsgType._Save_FriendChatInfo:
                    msgBytes = MessagePackSerializer.Serialize((MsgSave_FriendChatInfo)msg);
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

                case MsgType.EnterScene:
                    msgBytes = MessagePackSerializer.Serialize((MsgEnterScene)msg);
                    break;

                case MsgType.LeaveScene:
                    msgBytes = MessagePackSerializer.Serialize((MsgLeaveScene)msg);
                    break;

                case MsgType.SendSceneChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgSendSceneChat)msg);
                    break;

                case MsgType.AChatMessage:
                    msgBytes = MessagePackSerializer.Serialize((MsgAChatMessage)msg);
                    break;

                case MsgType.SearchScene:
                    msgBytes = MessagePackSerializer.Serialize((MsgSearchScene)msg);
                    break;

                case MsgType.GetRecommendedScenes:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetRecommendedScenes)msg);
                    break;

                case MsgType.SetName:
                    msgBytes = MessagePackSerializer.Serialize((MsgSetName)msg);
                    break;

                case MsgType.SetAvatarIndex:
                    msgBytes = MessagePackSerializer.Serialize((MsgSetAvatarIndex)msg);
                    break;

                case MsgType.GetSceneChatHistory:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetSceneChatHistory)msg);
                    break;

                case MsgType.ReportMessage:
                    msgBytes = MessagePackSerializer.Serialize((MsgReportMessage)msg);
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

                case MsgType.RemoveFriend:
                    msgBytes = MessagePackSerializer.Serialize((MsgRemoveFriend)msg);
                    break;

                case MsgType.AReceiveFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgAReceiveFriendRequest)msg);
                    break;

                case MsgType.AOtherAcceptFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgAOtherAcceptFriendRequest)msg);
                    break;

                case MsgType.AOtherRejectFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((MsgAOtherRejectFriendRequest)msg);
                    break;

                case MsgType.ARemoveFriend:
                    msgBytes = MessagePackSerializer.Serialize((MsgARemoveFriend)msg);
                    break;

                case MsgType.GetUserBriefInfos:
                    msgBytes = MessagePackSerializer.Serialize((MsgGetUserBriefInfos)msg);
                    break;

                case MsgType.SendFriendChat:
                    msgBytes = MessagePackSerializer.Serialize((MsgSendFriendChat)msg);
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