using Data;

namespace Script
{
    public class RoomManagerServiceProxy : ServiceProxy
    {
        public RoomManagerServiceProxy(Service self) : base(self, ServiceType.RoomManager)
        {
        }

        #region auto_proxy

        public async Task<MyResponse> LoadRoom(MsgRoomManagerLoadRoom msg)
        {
            return await this.Request(ServiceType.RoomManager, MsgType._RoomManager_LoadRoom, msg);
        }

        #endregion auto_proxy
    }
}