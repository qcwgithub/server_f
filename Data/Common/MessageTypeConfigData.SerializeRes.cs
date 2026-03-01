using MessagePack;

namespace Data
{
    public partial class MessageTypeConfigData
    {
        public static byte[] SerializeRes(MsgType msgType, object res)
        {
            if (res == null)
            {
                return null;
            }

            byte[] msgBytes;
            switch (msgType)
            {
                #region auto

                case MsgType._Timer:
                    msgBytes = MessagePackSerializer.Serialize((ResTimer)res);
                    break;

                case MsgType._Shutdown:
                    msgBytes = MessagePackSerializer.Serialize((ResShutdown)res);
                    break;

                case MsgType._ReloadScript:
                    msgBytes = MessagePackSerializer.Serialize((ResReloadScript)res);
                    break;

                case MsgType._ConnectorInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResConnectorInfo)res);
                    break;

                case MsgType._GetPendingMessageList:
                    msgBytes = MessagePackSerializer.Serialize((ResGetPendingMsgList)res);
                    break;

                case MsgType._GetScriptVersion:
                    msgBytes = MessagePackSerializer.Serialize((ResGetScriptVersion)res);
                    break;

                case MsgType._ReloadConfigs:
                    msgBytes = MessagePackSerializer.Serialize((ResReloadConfigs)res);
                    break;

                case MsgType._GC:
                    msgBytes = MessagePackSerializer.Serialize((ResGC)res);
                    break;

                case MsgType._RemoteWillShutdown:
                    msgBytes = MessagePackSerializer.Serialize((ResRemoteWillShutdown)res);
                    break;

                case MsgType._GetServiceState:
                    msgBytes = MessagePackSerializer.Serialize((ResGetServiceState)res);
                    break;

                case MsgType._GetReloadConfigOptions:
                    msgBytes = MessagePackSerializer.Serialize((ResGetReloadConfigOptions)res);
                    break;

                case MsgType._GetConnectedInfos:
                    msgBytes = MessagePackSerializer.Serialize((ResGetConnectedInfos)res);
                    break;

                case MsgType._ViewMongoDumpList:
                    msgBytes = MessagePackSerializer.Serialize((ResViewMongoDumpList)res);
                    break;

                case MsgType._A_ResGetServiceConfigs:
                    throw new Exception("Missing config for MsgType._A_ResGetServiceConfigs");

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

                case MsgType._User_ResetName:
                    msgBytes = MessagePackSerializer.Serialize((ResResetName)res);
                    break;

                case MsgType._User_ReceiveFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResReceiveFriendRequest)res);
                    break;

                case MsgType._User_OtherAcceptFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResOtherAcceptFriendRequest)res);
                    break;

                case MsgType._User_OtherRejectFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResOtherRejectFriendRequest)res);
                    break;

                case MsgType._User_OtherRemoveFriend:
                    msgBytes = MessagePackSerializer.Serialize((ResOtherRemoveFriend)res);
                    break;

                case MsgType._User_ReceiveChatMessage:
                    msgBytes = MessagePackSerializer.Serialize((ResReceiveChatMessage)res);
                    break;

                case MsgType._UserManager_UserLogin:
                    msgBytes = MessagePackSerializer.Serialize((ResUserManagerUserLogin)res);
                    break;

                case MsgType._UserManager_GetUserLocation:
                    msgBytes = MessagePackSerializer.Serialize((ResUserManagerGetUserLocation)res);
                    break;

                case MsgType._UserManager_ForwardToUserService:
                    msgBytes = MessagePackSerializer.Serialize((ResForwardToUserService)res);
                    break;

                case MsgType._Room_ServerAction:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomServiceAction)res);
                    break;

                case MsgType._Room_SaveSceneRoomInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveSceneRoomInfoToFile)res);
                    break;

                case MsgType._Room_UserEnterScene:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomUserEnterScene)res);
                    break;

                case MsgType._Room_UserLeaveScene:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomUserLeaveScene)res);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveRoomImmediately)res);
                    break;

                case MsgType._Room_SendSceneChat:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomSendSceneChat)res);
                    break;

                case MsgType._Room_SendFriendChat:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomSendFriendChat)res);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerLoadRoom)res);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerImportRoomConfig)res);
                    break;

                case MsgType._RoomManager_CreateFriendChatRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerCreateFriendChatRoom)res);
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

                case MsgType._Query_SceneRoomInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_SceneRoomInfo_by_roomId)res);
                    break;

                case MsgType._Query_SceneRoomInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_SceneRoomInfo_maxOf_roomId)res);
                    break;

                case MsgType._Insert_SceneRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_SceneRoomInfo)res);
                    break;

                case MsgType._Save_SceneRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_SceneRoomInfo)res);
                    break;

                case MsgType._Search_SceneRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSearch_SceneRoomInfo)res);
                    break;

                case MsgType._Save_MessageReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_MessageReportInfo)res);
                    break;

                case MsgType._Save_UserReportInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_UserReportInfo)res);
                    break;

                case MsgType._Save_UserBriefInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_UserBriefInfo)res);
                    break;

                case MsgType._Query_UserBriefInfo_by_userId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_UserBriefInfo_by_userId)res);
                    break;

                case MsgType._Query_FriendChatRoomInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_FriendChatRoomInfo_by_roomId)res);
                    break;

                case MsgType._Query_FriendChatRoomInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_FriendChatRoomInfo_maxOf_roomId)res);
                    break;

                case MsgType._Insert_FriendChatRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_FriendChatRoomInfo)res);
                    break;

                case MsgType._Save_FriendChatRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_FriendChatRoomInfo)res);
                    break;

                case MsgType._Query_FriendChatMessages_by_roomId_receivedSeqs:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_FriendChatMessages_by_roomId_receivedSeqs)res);
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

                case MsgType.EnterScene:
                    msgBytes = MessagePackSerializer.Serialize((ResEnterScene)res);
                    break;

                case MsgType.LeaveScene:
                    msgBytes = MessagePackSerializer.Serialize((ResLeaveScene)res);
                    break;

                case MsgType.SendSceneChat:
                    msgBytes = MessagePackSerializer.Serialize((ResSendSceneChat)res);
                    break;

                case MsgType.AChatMessage:
                    throw new Exception("Missing config for MsgType.AChatMessage");

                case MsgType.SearchScene:
                    msgBytes = MessagePackSerializer.Serialize((ResSearchScene)res);
                    break;

                case MsgType.GetRecommendedScenes:
                    msgBytes = MessagePackSerializer.Serialize((ResGetRecommendedScenes)res);
                    break;

                case MsgType.SetName:
                    msgBytes = MessagePackSerializer.Serialize((ResSetName)res);
                    break;

                case MsgType.SetAvatarIndex:
                    msgBytes = MessagePackSerializer.Serialize((ResSetAvatarIndex)res);
                    break;

                case MsgType.GetSceneChatHistory:
                    msgBytes = MessagePackSerializer.Serialize((ResGetSceneChatHistory)res);
                    break;

                case MsgType.ReportMessage:
                    msgBytes = MessagePackSerializer.Serialize((ResReportMessage)res);
                    break;

                case MsgType.ReportUser:
                    msgBytes = MessagePackSerializer.Serialize((ResReportUser)res);
                    break;

                case MsgType.SendFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResSendFriendRequest)res);
                    break;

                case MsgType.RejectFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResRejectFriendRequest)res);
                    break;

                case MsgType.AcceptFriendRequest:
                    msgBytes = MessagePackSerializer.Serialize((ResAcceptFriendRequest)res);
                    break;

                case MsgType.BlockUser:
                    msgBytes = MessagePackSerializer.Serialize((ResBlockUser)res);
                    break;

                case MsgType.UnblockUser:
                    msgBytes = MessagePackSerializer.Serialize((ResUnblockUser)res);
                    break;

                case MsgType.RemoveFriend:
                    msgBytes = MessagePackSerializer.Serialize((ResRemoveFriend)res);
                    break;

                case MsgType.AReceiveFriendRequest:
                    throw new Exception("Missing config for MsgType.AReceiveFriendRequest");

                case MsgType.AOtherAcceptFriendRequest:
                    throw new Exception("Missing config for MsgType.AOtherAcceptFriendRequest");

                case MsgType.AOtherRejectFriendRequest:
                    throw new Exception("Missing config for MsgType.AOtherRejectFriendRequest");

                case MsgType.ARemoveFriend:
                    throw new Exception("Missing config for MsgType.ARemoveFriend");

                case MsgType.GetUserBriefInfos:
                    msgBytes = MessagePackSerializer.Serialize((ResGetUserBriefInfos)res);
                    break;

                case MsgType.SendFriendChat:
                    msgBytes = MessagePackSerializer.Serialize((ResSendFriendChat)res);
                    break;

                case MsgType.ReceiveFriendChatMessages:
                    msgBytes = MessagePackSerializer.Serialize((ResReceiveFriendChatMessages)res);
                    break;

                case MsgType.SetFriendChatReadSeq:
                    msgBytes = MessagePackSerializer.Serialize((ResSetFriendChatReadSeq)res);
                    break;

                case MsgType.SetFriendChatReceivedSeq:
                    msgBytes = MessagePackSerializer.Serialize((ResSetFriendChatReceivedSeq)res);
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