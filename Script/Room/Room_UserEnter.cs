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
                this.logger.ErrorFormat("{0} userId {1} roomId {2}, room == null!", this.msgType, msg.userId, msg.roomId);
                return ECode.RoomNotExist;
            }

            return ECode.Success;
        }
    }
}