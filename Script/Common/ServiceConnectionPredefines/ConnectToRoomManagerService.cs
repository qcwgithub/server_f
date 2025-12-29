using Data;

namespace Script
{
    public class ConnectToRoomManagerService : ConnectToStatelessService
    {
        public ConnectToRoomManagerService(Service self) : base(self, ServiceType.RoomManager)
        {

        }

        public async Task<MyResponse<ResRoomManagerLoadRoom>> LoadRoom(MsgRoomManagerLoadRoom msg)
        {
            return await this.Request<MsgRoomManagerLoadRoom, ResRoomManagerLoadRoom>(MsgType._RoomManager_LoadRoom, msg);
        }
    }
}