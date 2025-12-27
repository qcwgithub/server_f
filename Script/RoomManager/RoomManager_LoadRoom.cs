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
                res.location = await this.service.roomLocator.GetLocation(msg.roomId);
                if (res.location.IsValid())
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

                res.location = await this.service.roomLocationAssignment.AssignLocation(roomInfo.roomId);
                if (!res.location.IsValid())
                {
                    return ECode.NoAvailableRoomService;
                }

                this.service.roomLocator.CacheLocation(roomInfo.roomId, res.location);
                return ECode.Success;
            }
            else
            {
                return ECode.Retry;
            }
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