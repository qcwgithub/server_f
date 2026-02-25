using Data;

namespace Script
{
    public class RoomServiceProxy : ServiceProxy
    {
        public RoomServiceProxy(Service self) : base(self, ServiceType.Room)
        {
        }

        #region auto_proxy

        public async Task<MyResponse> ServerAction(int serviceId, MsgRoomServiceAction msg)
        {
            return await this.Request(serviceId, MsgType._Room_ServerAction, msg);
        }
        public async Task<MyResponse> SaveSceneRoomInfoToFile(int serviceId, MsgSaveSceneRoomInfoToFile msg)
        {
            return await this.Request(serviceId, MsgType._Room_SaveSceneRoomInfoToFile, msg);
        }
        public async Task<MyResponse> UserEnterScene(int serviceId, MsgRoomUserEnterScene msg)
        {
            return await this.Request(serviceId, MsgType._Room_UserEnterScene, msg);
        }
        public async Task<MyResponse> UserLeaveScene(int serviceId, MsgRoomUserLeaveScene msg)
        {
            return await this.Request(serviceId, MsgType._Room_UserLeaveScene, msg);
        }
        public async Task<MyResponse> SaveRoomImmediately(int serviceId, MsgSaveRoomImmediately msg)
        {
            return await this.Request(serviceId, MsgType._Room_SaveRoomImmediately, msg);
        }
        public async Task<MyResponse> SendSceneChat(int serviceId, MsgRoomSendSceneChat msg)
        {
            return await this.Request(serviceId, MsgType._Room_SendSceneChat, msg);
        }
        public async Task<MyResponse> SendFriendChat(int serviceId, MsgRoomSendFriendChat msg)
        {
            return await this.Request(serviceId, MsgType._Room_SendFriendChat, msg);
        }

        #endregion auto_proxy
    }
}