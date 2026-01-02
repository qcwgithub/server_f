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
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                this.logger.ErrorFormat("{0} roomId {1} roomId {2}, room == null!", this.msgType, msg.roomId, msg.roomId);
                return ECode.RoomNotExist;
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomUserLeave msg, ECode e, ResRoomUserLeave res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}