using Data;

namespace Script
{
    public class Room_UserLeave : RoomHandler<MsgRoomUserLeave, ResRoomUserLeave>
    {
        public Room_UserLeave(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserLeave;
        public override async Task<ECode> Handle(MessageContext context, MsgRoomUserLeave msg, ResRoomUserLeave res)
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