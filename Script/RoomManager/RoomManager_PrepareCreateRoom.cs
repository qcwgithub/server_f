using Data;

namespace Script
{
    [AutoRegister]
    public class RoomManager_PrepareCreateRoom : Handler<RoomManagerService, MsgRoomManagerPrepareCreateRoom, ResRoomManagerPrepareCreateRoom>
    {
        public RoomManager_PrepareCreateRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_PrepareCreateRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerPrepareCreateRoom msg, ResRoomManagerPrepareCreateRoom res)
        {
            this.service.logger.Info($"{this.msgType} roomType {msg.roomType}");
            res.roomId = this.service.roomIdSnowflakeScript.NextRoomId();
            return ECode.Success;
        }
    }
}