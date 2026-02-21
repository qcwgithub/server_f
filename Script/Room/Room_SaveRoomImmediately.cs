using Data;
namespace Script
{
    [AutoRegister]
    public class Room_SaveRoomImmediately : Handler<RoomService, MsgSaveRoomImmediately, ResSaveRoomImmediately>
    {
        public Room_SaveRoomImmediately(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoomImmediately;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveRoomImmediately msg, ResSaveRoomImmediately res)
        {
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                this.service.logger.Error($"{this.msgType} roomId {msg.roomId} room == null");
                return ECode.RoomNotExist;
            }

            ECode e = await this.service.SaveSceneInfo(room, msg.reason);
            return e;
        }

        public override void PostHandle(MessageContext context, MsgSaveRoomImmediately msg, ECode e, ResSaveRoomImmediately res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}