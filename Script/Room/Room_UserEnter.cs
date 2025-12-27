using Data;

namespace Script
{
    public class Room_UserEnter : RoomHandler<MsgRoomUserEnter, ResRoomUserEnter>
    {
        public Room_UserEnter(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserEnter;
        public override async Task<ECode> Handle(IConnection connection, MsgRoomUserEnter msg, ResRoomUserEnter res)
        {
            Room? room = this.service.sd.GetRoom(msg.roomId);
            if (room == null)
            {
                // no, should get from room manager,
                (ECode e, RoomInfo? roomInfo) = await this.service.ss.QueryRoomInfo(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }

                if (roomInfo == null)
                {
                    
                }
            }

            return ECode.Success;
        }
    }
}