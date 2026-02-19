using Data;

namespace Script
{
    public partial class RoomService : Service
    {
        public RoomServiceData sd
        {
            get
            {
                return (RoomServiceData)this.data;
            }
        }

        //
        public readonly DbServiceProxy dbServiceProxy;
        public readonly GlobalServiceProxy globalServiceProxy;
        public readonly GatewayServiceProxy gatewayServiceProxy;
        public readonly RoomManagerServiceProxy roomManagerServiceProxy;

        protected override MessageDispatcher CreateMessageDispatcher()
        {
            return base.CreateMessageDispatcher();
        }

        public readonly RoomServiceScript ss;

        public RoomService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddServiceProxy(this.dbServiceProxy = new DbServiceProxy(this));
            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.AddServiceProxy(this.gatewayServiceProxy = new GatewayServiceProxy(this));
            this.AddServiceProxy(this.roomManagerServiceProxy = new RoomManagerServiceProxy(this));

            this.ss = new RoomServiceScript(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<RoomService>();
        }

        public override async Task Detach()
        {
            await base.Detach();
        }

        ServiceRuntimeInfo CreateRuntimeInfo()
        {
            var info = new ServiceRuntimeInfo();
            info.serviceId = this.serviceId;
            info.busyCount = this.sd.roomCount;
            info.allowNew = this.sd.allowNewRoom;
            return info;
        }

        public async Task UpdateRuntimeInfo()
        {
            await this.server.roomServiceRuntimeInfoRedisW.Update(this.CreateRuntimeInfo());
        }

        public async Task CheckUpdateRuntimeInfo()
        {
            if (Math.Abs(this.sd.roomCountDelta) >= 100)
            {
                this.sd.roomCountDelta = 0;
                await this.UpdateRuntimeInfo();
            }
        }

        public async Task<Room?> LockRoom(long roomId, object owner)
        {
            MyDebug.Assert(owner != null);

            Room? room = this.sd.GetRoom(roomId);

            if (!this.sd.lockedRoomDict.TryGetValue(roomId, out var lockedRoom))
            {
                this.sd.lockedRoomDict[roomId] = new RoomServiceData.LockedRoom
                {
                    owner = owner,
                };
                return room;
            }

            if (lockedRoom.owner == null)
            {
                lockedRoom.owner = owner;
                return room;
            }

            if (lockedRoom.owner == owner)
            {
                return room;
            }

            if (lockedRoom.waiting == null)
            {
                lockedRoom.waiting = new List<TaskCompletionSource>();
            }

            var tcs = new TaskCompletionSource();
            lockedRoom.waiting.Add(tcs);
            await tcs.Task;

            MyDebug.Assert(lockedRoom.owner == null);
            lockedRoom.owner = owner;
            return room;
        }

        public void TryUnlockRoom(long roomId, object owner)
        {
            MyDebug.Assert(owner != null);

            if (!this.sd.lockedRoomDict.TryGetValue(roomId, out var lockedRoom))
            {
                return;
            }

            if (lockedRoom.owner == owner)
            {
                lockedRoom.owner = null;
                this.sd.lockedRoomDict.Remove(roomId);

                if (lockedRoom.waiting != null && lockedRoom.waiting.Count > 0)
                {
                    var tcs = lockedRoom.waiting[0];
                    lockedRoom.waiting.RemoveAt(0);
                    tcs.SetResult();
                }
            }
            else
            {
                MyDebug.Assert(false);
            }
        }
    }
}