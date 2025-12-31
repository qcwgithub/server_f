using Data;

namespace Script
{
    public class RoomServiceProxy : ServiceProxy
    {
        public RoomServiceProxy(Service self) : base(self, ServiceType.Room)
        {
        }

        #region auto_proxy

        public async Task<MyResponse<ResRoomServiceAction>> ServerAction(int serviceId, MsgRoomServiceAction msg)
        {
            return await this.Request<MsgRoomServiceAction, ResRoomServiceAction>(serviceId, MsgType._Room_ServerAction, msg);
        }
        public async Task<MyResponse<ResSaveRoomInfoToFile>> SaveRoomInfoToFile(int serviceId, MsgSaveRoomInfoToFile msg)
        {
            return await this.Request<MsgSaveRoomInfoToFile, ResSaveRoomInfoToFile>(serviceId, MsgType._Room_SaveRoomInfoToFile, msg);
        }
        public async Task<MyResponse<ResRoomUserEnter>> UserEnter(int serviceId, MsgRoomUserEnter msg)
        {
            return await this.Request<MsgRoomUserEnter, ResRoomUserEnter>(serviceId, MsgType._Room_UserEnter, msg);
        }
        public async Task<MyResponse<ResRoomUserLeave>> UserLeave(int serviceId, MsgRoomUserLeave msg)
        {
            return await this.Request<MsgRoomUserLeave, ResRoomUserLeave>(serviceId, MsgType._Room_UserLeave, msg);
        }
        public async Task<MyResponse<ResRoomLoadRoom>> LoadRoom(int serviceId, MsgRoomLoadRoom msg)
        {
            return await this.Request<MsgRoomLoadRoom, ResRoomLoadRoom>(serviceId, MsgType._Room_LoadRoom, msg);
        }
        public async Task<MyResponse<ResSaveRoom>> SaveRoomImmediately(int serviceId, MsgSaveRoom msg)
        {
            return await this.Request<MsgSaveRoom, ResSaveRoom>(serviceId, MsgType._Room_SaveRoomImmediately, msg);
        }

        #endregion auto_proxy
    }
}