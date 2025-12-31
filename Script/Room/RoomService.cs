using Data;

namespace Script
{
    public class RoomService : Service
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

            this.ss = new RoomServiceScript(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<RoomService>();

            this.dispatcher.AddHandler(new Room_Action(this.server, this));
            this.dispatcher.AddHandler(new Room_DestroyRoom(this.server, this));
            this.dispatcher.AddHandler(new Room_OnReloadConfigs(this.server, this), true);
            this.dispatcher.AddHandler(new Room_SaveRoom(this.server, this));
            this.dispatcher.AddHandler(new Room_SaveRoomImmediately(this.server, this));
            this.dispatcher.AddHandler(new Room_SaveRoomInfoToFile(this.server, this));
            this.dispatcher.AddHandler(new Room_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new Room_Start(this.server, this));
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
    }
}