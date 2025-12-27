using Data;

namespace Script
{
    public class RoomManager_LoadRoom : RoomManagerHandler<MsgLoadRoom, ResLoadRoom>
    {
        public RoomManager_LoadRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_LoadRoom;

        protected override async Task<ECode> Handle(ServiceConnection connection, MsgLoadRoom msg, ResLoadRoom res)
        {
            msg.lockValue = await this.server.lockRedis.LockRoom(msg.roomId, this.service.logger);
            if (msg.lockValue != null)
            {
                (res.serviceId, res.expiry) = await this.service.roomLocator.GetOwningServiceIdWithExpiry(msg.roomId);
                if (res.serviceId != 0)
                {
                    return ECode.Success;
                }

                (ECode e, RoomInfo? roomInfo) = await this.service.ss.QueryRoomInfo(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }

                if (roomInfo == null)
                {
                    return ECode.RoomNotExist;
                }

                res.serviceId = await this.service.roomLocationAssignment.AssignServiceId(roomInfo.roomId);
                if (res.serviceId == 0)
                {
                    return ECode.NoAvailableRoomService;
                }

                res.expiry = TimeUtils.GetTimeS() + 60;
                this.service.roomLocator.SaveOwningServiceId(roomInfo.roomId, res.serviceId, res.expiry);
                return ECode.Success;
            }
            else
            {
                // Just wait
            }

            return ECode.Success;
        }

        public override void PostHandle(IConnection connection, MsgLoadRoom msg, ECode e, ResLoadRoom res)
        {
            if (msg.lockValue != null)
            {
                this.server.lockRedis.UnlockRoom(msg.roomId, msg.lockValue).Forget(this.service);
            }
        }
    }
}