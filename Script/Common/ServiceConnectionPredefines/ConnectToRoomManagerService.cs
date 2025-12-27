using Data;

namespace Script
{
    public class ConnectToRoomManagerService : ConnectToStatelessService
    {
        public ConnectToRoomManagerService(Service self) : base(self, ServiceType.RoomManager)
        {

        }
    }
}