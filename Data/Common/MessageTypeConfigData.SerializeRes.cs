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

                case MsgType._Room_SaveSceneInfoToFile:
                    msgBytes = MessagePackSerializer.Serialize((ResSaveSceneInfoToFile)res);
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

                case MsgType._Room_SendPrivateChat:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomSendPrivateChat)res);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerLoadRoom)res);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerImportRoomConfig)res);
                    break;

                case MsgType._RoomManager_CreatePrivateRoom:
                    msgBytes = MessagePackSerializer.Serialize((ResRoomManagerCreatePrivateRoom)res);
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

                case MsgType._Query_SceneInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_SceneInfo_by_roomId)res);
                    break;

                case MsgType._Query_SceneInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_SceneInfo_maxOf_roomId)res);
                    break;

                case MsgType._Insert_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_SceneInfo)res);
                    break;

                case MsgType._Save_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_SceneInfo)res);
                    break;

                case MsgType._Search_SceneInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSearch_SceneInfo)res);
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

                case MsgType._Query_PrivateRoomInfo_by_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_PrivateRoomInfo_by_roomId)res);
                    break;

                case MsgType._Query_PrivateRoomInfo_maxOf_roomId:
                    msgBytes = MessagePackSerializer.Serialize((ResQuery_PrivateRoomInfo_maxOf_roomId)res);
                    break;

                case MsgType._Insert_PrivateRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResInsert_PrivateRoomInfo)res);
                    break;

                case MsgType._Save_PrivateRoomInfo:
                    msgBytes = MessagePackSerializer.Serialize((ResSave_PrivateRoomInfo)res);
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

                case MsgType.SendPrivateChat:
                    msgBytes = MessagePackSerializer.Serialize((ResSendPrivateChat)res);
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