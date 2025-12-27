using Data;

namespace Script
{
    public class RoomManagerService : Service
    {
        public RoomManagerServiceData sd
        {
            get
            {
                return (RoomManagerServiceData)this.data;
            }
        }

        public readonly ConnectToDbService connectToDbService;
        public readonly ConnectToGlobalService connectToGlobalService;
        public readonly ConnectToGatewayService connectToGatewayService;
        public readonly RoomIdSnowflakeScript roomIdSnowflakeScript;
        public readonly RoomManagerServiceScript ss;
        public readonly ObjectLocator roomLocator;
        public readonly ObjectLocationAssignment roomLocationAssignment;

        public RoomManagerService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddConnectToOtherService(this.connectToDbService = new ConnectToDbService(this));
            this.AddConnectToOtherService(this.connectToGlobalService = new ConnectToGlobalService(this));
            this.AddConnectToOtherService(this.connectToGatewayService = new ConnectToGatewayService(this));

            this.roomIdSnowflakeScript = new RoomIdSnowflakeScript(this.server, this);
            this.ss = new RoomManagerServiceScript(this.server, this);
            this.roomLocator = ObjectLocator.CreateRoomLocator(this.server, this, this.sd.roomLocatorData);
            this.roomLocationAssignment = ObjectLocationAssignment.CreateRoomLocationAssignment(this.server, this, this.sd.roomLocationAssignmentData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<RoomManagerService>();

            this.dispatcher.AddHandler(new RoomManager_Start(this.server, this));
            this.dispatcher.AddHandler(new RoomManager_Shutdown(this.server, this));
        }
    }
}