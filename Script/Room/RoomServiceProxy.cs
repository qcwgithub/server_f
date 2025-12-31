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
        public async Task<MyResponse> LoadRoom(int serviceId, MsgRoomLoadRoom msg)
        {
            return await this.Request(serviceId, MsgType._Room_LoadRoom, msg);
        }
        public async Task<MyResponse> SaveRoomImmediately(int serviceId, MsgSaveRoom msg)
        {
            return await this.Request(serviceId, MsgType._Room_SaveRoomImmediately, msg);
        }

        #endregion auto_proxy
    }
}