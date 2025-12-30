using Data;

namespace Script
{
    public class ConnectWithRoomService : ConnectToStatefulService
    {
        public ConnectWithRoomService(Service self) : base(self, ServiceType.Room)
        {

        }

        #region auto_request

        #endregion auto_request

        public async Task<MyResponse<ResRoomUserLeave>> UserLeave(int serviceId, MsgRoomUserLeave msg)
        {
            return await this.Request<MsgRoomUserLeave, ResRoomUserLeave>(serviceId, MsgType._Room_UserLeave, msg);
        }
    }

    public class ConnectFromRoomService : ConnectWithRoomService
    {
        public ConnectFromRoomService(Service self) : base(self)
        {

        }
    }

    public class ConnectToRoomService : ConnectWithRoomService
    {
        public ConnectToRoomService(Service self) : base(self)
        {

        }
    }
}