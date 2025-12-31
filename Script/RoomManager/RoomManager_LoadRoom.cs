using Data;

namespace Script
{
    public class RoomManager_LoadRoom : RoomManagerHandler<MsgRoomManagerLoadRoom, ResRoomManagerLoadRoom>
    {
        public RoomManager_LoadRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_LoadRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerLoadRoom msg, ResRoomManagerLoadRoom res)
        {
            msg.lockValue = await this.server.lockRedis.LockRoom(msg.roomId, this.service.logger);
            if (msg.lockValue != null)
            {
                return ECode.Retry;
            }

            stObjectLocation location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (!location.IsValid())
            {
                location = await this.service.roomLocationAssignment.AssignLocation(msg.roomId);
                if (!location.IsValid())
                {
                    return ECode.NoAvailableRoomService;
                }

                this.service.roomLocator.CacheLocation(msg.roomId, res.location);
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomManagerLoadRoom msg, ECode e, ResRoomManagerLoadRoom res)
        {
            if (msg.lockValue != null)
            {
                this.server.lockRedis.UnlockRoom(msg.roomId, msg.lockValue).Forget(this.service);
            }
        }
    }
}