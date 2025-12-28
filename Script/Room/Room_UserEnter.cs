using Data;

namespace Script
{
    public class Room_UserEnter : RoomHandler<MsgRoomUserEnter, ResRoomUserEnter>
    {
        public Room_UserEnter(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserEnter;
        protected override async Task<ECode> Handle(ServiceConnection connection, MsgRoomUserEnter msg, ResRoomUserEnter res)
        {
            Room? room = this.service.sd.GetRoom(msg.roomId);
            if (room == null)
            {
                return ECode.RoomNotExist;
            }

            return ECode.Success;
        }
    }
}