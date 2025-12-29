using Data;

namespace Script
{
    public class ConnectToRoomService : ConnectToStatefulService
    {
        public ConnectToRoomService(Service self) : base(self, ServiceType.Room)
        {

        }

        public async Task<MyResponse<ResRoomUserLeave>> UserLeave(int serviceId, MsgRoomUserLeave msg)
        {
            return await this.Request<MsgRoomUserLeave, ResRoomUserLeave>(serviceId, MsgType._Room_UserLeave, msg);
        }
    }
}