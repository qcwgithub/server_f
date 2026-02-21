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

                case MsgType._User_ReceiveFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgReceiveFriendRequest>(msgBytes);
                    break;

                case MsgType._User_OtherAcceptFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgOtherAcceptFriendRequest>(msgBytes);
                    break;

                case MsgType._User_OtherRejectFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgOtherRejectFriendRequest>(msgBytes);
                    break;

                case MsgType._User_OtherRemoveFriend:
                    ob = MessagePackSerializer.Deserialize<MsgOtherRemoveFriend>(msgBytes);
                    break;

                case MsgType._UserManager_UserLogin:
                    ob = MessagePackSerializer.Deserialize<MsgUserManagerUserLogin>(msgBytes);
                    break;

                case MsgType._UserManager_GetUserLocation:
                    ob = MessagePackSerializer.Deserialize<MsgUserManagerGetUserLocation>(msgBytes);
                    break;

                case MsgType._UserManager_ForwardToUserService:
                    ob = MessagePackSerializer.Deserialize<MsgForwardToUserService>(msgBytes);
                    break;

                case MsgType._Room_ServerAction:
                    ob = MessagePackSerializer.Deserialize<MsgRoomServiceAction>(msgBytes);
                    break;

                case MsgType._Room_SaveSceneInfoToFile:
                    ob = MessagePackSerializer.Deserialize<MsgSaveSceneInfoToFile>(msgBytes);
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

                case MsgType._Room_SendSceneChat:
                    ob = MessagePackSerializer.Deserialize<MsgRoomSendSceneChat>(msgBytes);
                    break;

                case MsgType._Room_SendPrivateChat:
                    ob = MessagePackSerializer.Deserialize<MsgRoomSendPrivateChat>(msgBytes);
                    break;

                case MsgType._RoomManager_LoadRoom:
                    ob = MessagePackSerializer.Deserialize<MsgRoomManagerLoadRoom>(msgBytes);
                    break;

                case MsgType._RoomManager_ImportRoomConfig:
                    ob = MessagePackSerializer.Deserialize<MsgRoomManagerImportRoomConfig>(msgBytes);
                    break;

                case MsgType._RoomManager_CreatePrivateRoom:
                    ob = MessagePackSerializer.Deserialize<MsgRoomManagerCreatePrivateRoom>(msgBytes);
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

                case MsgType._Query_SceneInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_SceneInfo_by_roomId>(msgBytes);
                    break;

                case MsgType._Query_SceneInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_SceneInfo_maxOf_roomId>(msgBytes);
                    break;

                case MsgType._Insert_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<MsgInsert_SceneInfo>(msgBytes);
                    break;

                case MsgType._Save_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_SceneInfo>(msgBytes);
                    break;

                case MsgType._Search_SceneInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSearch_SceneInfo>(msgBytes);
                    break;

                case MsgType._Save_MessageReportInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_MessageReportInfo>(msgBytes);
                    break;

                case MsgType._Save_UserReportInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_UserReportInfo>(msgBytes);
                    break;

                case MsgType._Save_UserBriefInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_UserBriefInfo>(msgBytes);
                    break;

                case MsgType._Query_UserBriefInfo_by_userId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_UserBriefInfo_by_userId>(msgBytes);
                    break;

                case MsgType._Query_PrivateRoomInfo_by_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_PrivateRoomInfo_by_roomId>(msgBytes);
                    break;

                case MsgType._Query_PrivateRoomInfo_maxOf_roomId:
                    ob = MessagePackSerializer.Deserialize<MsgQuery_PrivateRoomInfo_maxOf_roomId>(msgBytes);
                    break;

                case MsgType._Insert_PrivateRoomInfo:
                    ob = MessagePackSerializer.Deserialize<MsgInsert_PrivateRoomInfo>(msgBytes);
                    break;

                case MsgType._Save_PrivateRoomInfo:
                    ob = MessagePackSerializer.Deserialize<MsgSave_PrivateRoomInfo>(msgBytes);
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

                case MsgType.EnterScene:
                    ob = MessagePackSerializer.Deserialize<MsgEnterScene>(msgBytes);
                    break;

                case MsgType.LeaveScene:
                    ob = MessagePackSerializer.Deserialize<MsgLeaveScene>(msgBytes);
                    break;

                case MsgType.SendSceneChat:
                    ob = MessagePackSerializer.Deserialize<MsgSendSceneChat>(msgBytes);
                    break;

                case MsgType.AChatMessage:
                    ob = MessagePackSerializer.Deserialize<MsgAChatMessage>(msgBytes);
                    break;

                case MsgType.SearchScene:
                    ob = MessagePackSerializer.Deserialize<MsgSearchScene>(msgBytes);
                    break;

                case MsgType.GetRecommendedScenes:
                    ob = MessagePackSerializer.Deserialize<MsgGetRecommendedScenes>(msgBytes);
                    break;

                case MsgType.SetName:
                    ob = MessagePackSerializer.Deserialize<MsgSetName>(msgBytes);
                    break;

                case MsgType.SetAvatarIndex:
                    ob = MessagePackSerializer.Deserialize<MsgSetAvatarIndex>(msgBytes);
                    break;

                case MsgType.GetSceneChatHistory:
                    ob = MessagePackSerializer.Deserialize<MsgGetSceneChatHistory>(msgBytes);
                    break;

                case MsgType.ReportMessage:
                    ob = MessagePackSerializer.Deserialize<MsgReportMessage>(msgBytes);
                    break;

                case MsgType.ReportUser:
                    ob = MessagePackSerializer.Deserialize<MsgReportUser>(msgBytes);
                    break;

                case MsgType.SendFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgSendFriendRequest>(msgBytes);
                    break;

                case MsgType.RejectFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgRejectFriendRequest>(msgBytes);
                    break;

                case MsgType.AcceptFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgAcceptFriendRequest>(msgBytes);
                    break;

                case MsgType.BlockUser:
                    ob = MessagePackSerializer.Deserialize<MsgBlockUser>(msgBytes);
                    break;

                case MsgType.UnblockUser:
                    ob = MessagePackSerializer.Deserialize<MsgUnblockUser>(msgBytes);
                    break;

                case MsgType.RemoveFriend:
                    ob = MessagePackSerializer.Deserialize<MsgRemoveFriend>(msgBytes);
                    break;

                case MsgType.AReceiveFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgAReceiveFriendRequest>(msgBytes);
                    break;

                case MsgType.AOtherAcceptFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgAOtherAcceptFriendRequest>(msgBytes);
                    break;

                case MsgType.AOtherRejectFriendRequest:
                    ob = MessagePackSerializer.Deserialize<MsgAOtherRejectFriendRequest>(msgBytes);
                    break;

                case MsgType.ARemoveFriend:
                    ob = MessagePackSerializer.Deserialize<MsgARemoveFriend>(msgBytes);
                    break;

                case MsgType.GetUserBriefInfos:
                    ob = MessagePackSerializer.Deserialize<MsgGetUserBriefInfos>(msgBytes);
                    break;

                case MsgType.SendPrivateChat:
                    ob = MessagePackSerializer.Deserialize<MsgSendPrivateChat>(msgBytes);
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