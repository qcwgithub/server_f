
using Data;

namespace Script
{
    public partial class RoomManagerService : Service
    {
        public RoomManagerServiceData sd
        {
            get
            {
                return (RoomManagerServiceData)this.data;
            }
        }

        public readonly DbServiceProxy dbServiceProxy;
        public readonly GlobalServiceProxy globalServiceProxy;
        public readonly GatewayServiceProxy gatewayServiceProxy;
        public readonly RoomIdSnowflakeScript roomIdSnowflakeScript;
        public readonly RoomManagerServiceScript ss;
        public readonly RoomManagerRoomScript roomScript;
        public readonly ObjectLocator roomLocator;
        public readonly ObjectLocationAssignment roomLocationAssignment;
        public readonly RoomServiceProxy roomServiceProxy;

        public RoomManagerService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddServiceProxy(this.dbServiceProxy = new DbServiceProxy(this));
            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.AddServiceProxy(this.gatewayServiceProxy = new GatewayServiceProxy(this));
            this.AddServiceProxy(this.roomServiceProxy = new RoomServiceProxy(this));

            this.roomIdSnowflakeScript = new RoomIdSnowflakeScript(this.server, this);
            this.ss = new RoomManagerServiceScript(this.server, this);
            this.roomScript = new RoomManagerRoomScript(this.server, this);
            this.roomLocator = ObjectLocator.CreateRoomLocator(this.server, this, this.sd.roomLocatorData);
            this.roomLocationAssignment = ObjectLocationAssignment.CreateRoomLocationAssignment(this.server, this, this.sd.roomLocationAssignmentData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<RoomManagerService>();
        }
    }
}