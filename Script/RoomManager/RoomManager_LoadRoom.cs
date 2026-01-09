using Data;

namespace Script
{
    public class RoomManager_LoadRoom : Handler<RoomManagerService, MsgRoomManagerLoadRoom, ResRoomManagerLoadRoom>
    {
        public RoomManager_LoadRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_LoadRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerLoadRoom msg, ResRoomManagerLoadRoom res)
        {
            this.service.logger.Info($"{this.msgType} roomId {msg.roomId}");

            if (!SnowflakeScript<Service>.CheckValid(msg.roomId))
            {
                return ECode.InvalidRoomId;
            }

            context.lockValue = await this.server.lockRedis.LockRoom(msg.roomId, this.service.logger);
            if (context.lockValue == null)
            {
                return ECode.Retry;
            }

            res.location = await this.service.roomLocator.GetLocation(msg.roomId);
            if (res.location.IsValid())
            {
                return ECode.Success;
            }

            res.location = await this.service.roomLocationAssignment.AssignLocation(msg.roomId);
            if (!res.location.IsValid())
            {
                return ECode.NoAvailableRoomService;
            }

            this.service.roomLocator.CacheLocation(msg.roomId, res.location);

            var msgR = new MsgRoomLoadRoom();
            msgR.roomId = msg.roomId;

            var r = await this.service.roomServiceProxy.LoadRoom(res.location.serviceId, msgR);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            // var resR = r.CastRes<ResRoomLoadRoom>();

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomManagerLoadRoom msg, ECode e, ResRoomManagerLoadRoom res)
        {
            if (context.lockValue != null)
            {
                this.server.lockRedis.UnlockRoom(msg.roomId, context.lockValue).Forget(this.service);
            }
        }
    }
}