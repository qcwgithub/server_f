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
        public async Task<MyResponse> SaveRoomInfoToFile(int serviceId, MsgSaveRoomInfoToFile msg)
        {
            return await this.Request(serviceId, MsgType._Room_SaveRoomInfoToFile, msg);
        }
        public async Task<MyResponse> UserEnter(int serviceId, MsgRoomUserEnter msg)
        {
            return await this.Request(serviceId, MsgType._Room_UserEnter, msg);
        }
        public async Task<MyResponse> UserLeave(int serviceId, MsgRoomUserLeave msg)
        {
            return await this.Request(serviceId, MsgType._Room_UserLeave, msg);
        }
        public async Task<MyResponse> SaveRoomImmediately(int serviceId, MsgSaveRoomImmediately msg)
        {
            return await this.Request(serviceId, MsgType._Room_SaveRoomImmediately, msg);
        }
        public async Task<MyResponse> SendSceneChat(int serviceId, MsgRoomSendSceneChat msg)
        {
            return await this.Request(serviceId, MsgType._Room_SendSceneChat, msg);
        }
        public async Task<MyResponse> SendPrivateChat(int serviceId, MsgRoomSendPrivateChat msg)
        {
            return await this.Request(serviceId, MsgType._Room_SendPrivateChat, msg);
        }

        #endregion auto_proxy
    }
}