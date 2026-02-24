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

                case MsgType._Timer:
                    ob = MessagePackSerializer.Deserialize<ResTimer>(resBytes);
                    break;

                case MsgType._Shutdown:
                    ob = MessagePackSerializer.Deserialize<ResShutdown>(resBytes);
                    break;

                case MsgType._ReloadScript:
                    ob = MessagePackSerializer.Deserialize<ResReloadScript>(resBytes);
                    break;

                case MsgType._ConnectorInfo:
                    ob = MessagePackSerializer.Deserialize<ResConnectorInfo>(resBytes);
                    break;

                case MsgType._GetPendingMessageList:
                    ob = MessagePackSerializer.Deserialize<ResGetPendingMsgList>(resBytes);
                    break;

                case MsgType._GetScriptVersion:
                    ob = MessagePackSerializer.Deserialize<ResGetScriptVersion>(resBytes);
                    break;

                case MsgType._ReloadConfigs:
                    ob = MessagePackSerializer.Deserialize<ResReloadConfigs>(resBytes);
                    break;

                case MsgType._GC:
                    ob = MessagePackSerializer.Deserialize<ResGC>(resBytes);
                    break;

                case MsgType._RemoteWillShutdown:
                    ob = MessagePackSerializer.Deserialize<ResRemoteWillShutdown>(resBytes);
                    break;

                case MsgType._GetServiceState:
                    ob = MessagePackSerializer.Deserialize<ResGetServiceState>(resBytes);
                    break;

                case MsgType._GetReloadConfigOptions:
                    ob = MessagePackSerializer.Deserialize<ResGetReloadConfigOptions>(resBytes);
                    break;

                case MsgType._GetConnectedInfos:
                    ob = MessagePackSerializer.Deserialize<ResGetConnectedInfos>(resBytes);
                    break;

                case MsgType._ViewMongoDumpList:
                    ob = MessagePackSerializer.Deserialize<ResViewMongoDumpList>(resBytes);
                    break;

                case MsgType._A_ResGetServiceConfigs:
                    throw new Exception("Missing config for MsgType._A_ResGetServiceConfigs");

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

                case MsgType._User_ResetName:
                    ob = MessagePackSerializer.Deserialize<ResResetName>(resBytes);
                    break;

                case MsgType._User_ReceiveFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResReceiveFriendRequest>(resBytes);
                    break;

                case MsgType._User_OtherAcceptFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResOtherAcceptFriendRequest>(resBytes);
                    break;

                case MsgType._User_OtherRejectFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResOtherRejectFriendRequest>(resBytes);
                    break;

                case MsgType._User_OtherRemoveFriend:
                    ob = MessagePackSerializer.Deserialize<ResOtherRemoveFriend>(resBytes);
                    break;

                case MsgType._User_ReceiveChatMessage:
                    ob = MessagePackSerializer.Deserialize<ResReceiveChatMessage>(resBytes);
                    break;

                case MsgType._UserManager_UserLogin:
                    ob = MessagePackSerializer.Deserialize<ResUserManagerUserLogin>(resBytes);
                    break;

                case MsgType._UserManager_GetUserLocation:
                    ob = MessagePackSerializer.Deserialize<ResUserManagerGetUserLocation>(resBytes);
                    break;

                case MsgType._UserManager_ForwardToUserService:
                    ob = MessagePackSerializer.Deserialize<ResForwardToUserService>(resBytes);
                    break;

                case MsgType._Room_ServerAction:
                    ob = MessagePackSerializer.Deserialize<ResRoomServiceAction>(resBytes);
                    break;

                case MsgType._Room_SaveSceneInfoToFile:
                    ob = MessagePackSerializer.Deserialize<ResSaveSceneInfoToFile>(resBytes);
                    break;

                case MsgType._Room_UserEnterScene:
                    ob = MessagePackSerializer.Deserialize<ResRoomUserEnterScene>(resBytes);
                    break;

                case MsgType._Room_UserLeaveScene:
                    ob = MessagePackSerializer.Deserialize<ResRoomUserLeaveScene>(resBytes);
                    break;

                case MsgType._Room_SaveRoomImmediately:
                    ob = MessagePackSerializer.Deserialize<ResSaveRoomImmediately>(resBytes);
                    break;

                case MsgType._Room_SendSceneChat:
                    ob = MessagePackSerializer.Deserialize<ResRoomSendSceneChat>(resBytes);
                    break;

                case MsgType._Room_SendFriendChat:
                    ob = MessagePackSerializer.Deserialize<ResRoomSendFriendChat>(resBytes);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    ob = MessagePackSerializer.Deserialize<ResRoomManagerLoadRoom>(resBytes);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    ob = MessagePackSerializer.Deserialize<ResRoomManagerImportRoomConfig>(resBytes);
                    break;

                case MsgType._RoomManager_CreateFriendChatRoom:
                    ob = MessagePackSerializer.Deserialize<ResRoomManagerCreateFriendChatRoom>(resBytes);
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

                case MsgType._Query_SceneInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_SceneInfo_by_roomId>(resBytes);
                    break;

                case MsgType._Query_SceneInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_SceneInfo_maxOf_roomId>(resBytes);
                    break;

                case MsgType._Insert_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<ResInsert_SceneInfo>(resBytes);
                    break;

                case MsgType._Save_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_SceneInfo>(resBytes);
                    break;

                case MsgType._Search_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<ResSearch_SceneInfo>(resBytes);
                    break;

                case MsgType._Save_MessageReportInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_MessageReportInfo>(resBytes);
                    break;

                case MsgType._Save_UserReportInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_UserReportInfo>(resBytes);
                    break;

                case MsgType._Save_UserBriefInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_UserBriefInfo>(resBytes);
                    break;

                case MsgType._Query_UserBriefInfo_by_userId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_UserBriefInfo_by_userId>(resBytes);
                    break;

                case MsgType._Query_FriendChatInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_FriendChatInfo_by_roomId>(resBytes);
                    break;

                case MsgType._Query_FriendChatInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_FriendChatInfo_maxOf_roomId>(resBytes);
                    break;

                case MsgType._Insert_FriendChatInfo:
                    ob = MessagePackSerializer.Deserialize<ResInsert_FriendChatInfo>(resBytes);
                    break;

                case MsgType._Save_FriendChatInfo:
                    ob = MessagePackSerializer.Deserialize<ResSave_FriendChatInfo>(resBytes);
                    break;

                case MsgType._Save_UserFriendChatState:
                    ob = MessagePackSerializer.Deserialize<ResSave_UserFriendChatState>(resBytes);
                    break;

                case MsgType._Query_UserFriendChatState_by_userId:
                    ob = MessagePackSerializer.Deserialize<ResQuery_UserFriendChatState_by_userId>(resBytes);
                    break;

                case MsgType._Query_FriendChatMessages_by_roomId_readSeqs:
                    ob = MessagePackSerializer.Deserialize<ResQuery_FriendChatMessages_by_roomId_readSeqs>(resBytes);
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

                case MsgType.EnterScene:
                    ob = MessagePackSerializer.Deserialize<ResEnterScene>(resBytes);
                    break;

                case MsgType.LeaveScene:
                    ob = MessagePackSerializer.Deserialize<ResLeaveScene>(resBytes);
                    break;

                case MsgType.SendSceneChat:
                    ob = MessagePackSerializer.Deserialize<ResSendSceneChat>(resBytes);
                    break;

                case MsgType.AChatMessage:
                    throw new Exception("Missing config for MsgType.AChatMessage");

                case MsgType.SearchScene:
                    ob = MessagePackSerializer.Deserialize<ResSearchScene>(resBytes);
                    break;

                case MsgType.GetRecommendedScenes:
                    ob = MessagePackSerializer.Deserialize<ResGetRecommendedScenes>(resBytes);
                    break;

                case MsgType.SetName:
                    ob = MessagePackSerializer.Deserialize<ResSetName>(resBytes);
                    break;

                case MsgType.SetAvatarIndex:
                    ob = MessagePackSerializer.Deserialize<ResSetAvatarIndex>(resBytes);
                    break;

                case MsgType.GetSceneChatHistory:
                    ob = MessagePackSerializer.Deserialize<ResGetSceneChatHistory>(resBytes);
                    break;

                case MsgType.ReportMessage:
                    ob = MessagePackSerializer.Deserialize<ResReportMessage>(resBytes);
                    break;

                case MsgType.ReportUser:
                    ob = MessagePackSerializer.Deserialize<ResReportUser>(resBytes);
                    break;

                case MsgType.SendFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResSendFriendRequest>(resBytes);
                    break;

                case MsgType.RejectFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResRejectFriendRequest>(resBytes);
                    break;

                case MsgType.AcceptFriendRequest:
                    ob = MessagePackSerializer.Deserialize<ResAcceptFriendRequest>(resBytes);
                    break;

                case MsgType.BlockUser:
                    ob = MessagePackSerializer.Deserialize<ResBlockUser>(resBytes);
                    break;

                case MsgType.UnblockUser:
                    ob = MessagePackSerializer.Deserialize<ResUnblockUser>(resBytes);
                    break;

                case MsgType.RemoveFriend:
                    ob = MessagePackSerializer.Deserialize<ResRemoveFriend>(resBytes);
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
                    ob = MessagePackSerializer.Deserialize<ResGetUserBriefInfos>(resBytes);
                    break;

                case MsgType.SendFriendChat:
                    ob = MessagePackSerializer.Deserialize<ResSendFriendChat>(resBytes);
                    break;

                case MsgType.GetFriendChatUnreadMessages:
                    ob = MessagePackSerializer.Deserialize<ResGetFriendChatUnreadMessages>(resBytes);
                    break;

                case MsgType.AckFriendChatReadSeq1:
                    ob = MessagePackSerializer.Deserialize<ResAckFriendChatReadSeq1>(resBytes);
                    break;

                case MsgType.AckFriendChatReadSeqN:
                    ob = MessagePackSerializer.Deserialize<ResAckFriendChatReadSeqN>(resBytes);
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