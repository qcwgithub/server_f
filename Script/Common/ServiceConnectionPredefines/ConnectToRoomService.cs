using Data;

namespace Script
{
    public class ConnectToRoomService : ConnectToStatefulService
    {
        public ConnectToRoomService(Service self) : base(self, ServiceType.Room)
        {

        }
    }
}