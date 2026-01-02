using Data;

namespace Script
{
    public class Room_LoadRoom : RoomHandler<MsgRoomLoadRoom, ResRoomLoadRoom>
    {
        public Room_LoadRoom(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_LoadRoom;
        public override async Task<ECode> Handle(MessageContext context, MsgRoomLoadRoom msg, ResRoomLoadRoom res)
        {
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                (ECode e, RoomInfo? roomInfo) = await this.service.ss.QueryRoomInfo(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }

                if (roomInfo == null)
                {
                    return ECode.RoomNotExist;
                }

                room = new Room(roomInfo);

                await this.server.roomLocationRedisW.WriteLocation(msg.roomId, this.service.serviceId, this.service.sd.saveIntervalS + 60);

                this.AddRoomToDict(room);
            }

            if (!room.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(room);
            }

            this.service.ss.ClearDestroyTimer(room, RoomClearDestroyTimerReason.RoomLoginSuccess);

            res.roomInfo = room.roomInfo;
            return ECode.Success;
        }

        void AddRoomToDict(Room room)
        {
            // runtime 初始化
            this.service.sd.AddRoom(room);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(room.lastRoomInfo == null);

            room.lastRoomInfo = RoomInfo.Ensure(null);
            room.lastRoomInfo.DeepCopyFrom(room.roomInfo);

            // qiucw
            // 这句会修改 roomInfo，必须放在 lastRoomInfo.DeepCopyFrom 后面
            // this.gameScripts.CallInit(room);
            this.service.CheckUpdateRuntimeInfo().Forget();
        }

        public override void PostHandle(MessageContext context, MsgRoomLoadRoom msg, ECode e, ResRoomLoadRoom res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}