using Data;

namespace Script
{
    public class UserService : Service
    {
        public UserServiceData sd
        {
            get
            {
                return (UserServiceData)this.data;
            }
        }

        //
        public readonly DbServiceProxy dbServiceProxy;
        public readonly GlobalServiceProxy globalServiceProxy;
        public readonly GatewayServiceProxy gatewayServiceProxy;
        public readonly RoomServiceProxy roomServiceProxy;
        public readonly RoomManagerServiceProxy roomManagerServiceProxy;

        protected override MessageDispatcher CreateMessageDispatcher()
        {
            return new UserMessageDispatcher(this.server, this);
        }

        public readonly UserServiceScript ss;
        public readonly ObjectLocator roomLocator;

        public UserService(Server server, int serviceId) : base(server, serviceId)
        {
            //
            this.AddServiceProxy(this.dbServiceProxy = new DbServiceProxy(this));
            this.AddServiceProxy(this.globalServiceProxy = new GlobalServiceProxy(this));
            this.AddServiceProxy(this.gatewayServiceProxy = new GatewayServiceProxy(this));
            this.AddServiceProxy(this.roomServiceProxy = new RoomServiceProxy(this));
            this.AddServiceProxy(this.roomManagerServiceProxy = new RoomManagerServiceProxy(this));

            this.ss = new UserServiceScript(this.server, this);

            this.roomLocator = ObjectLocator.CreateRoomLocator(this.server, this, this.sd.roomLocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserService>();

            this.dispatcher.AddHandler(new User_Action(this.server, this));
            this.dispatcher.AddHandler(new User_DestroyUser(this.server, this));
            this.dispatcher.AddHandler(new User_OnConnectComplete(this.server, this), true);
            this.dispatcher.AddHandler(new User_OnConnectionClose(this.server, this), true);
            this.dispatcher.AddHandler(new User_OnReloadConfigs(this.server, this), true);
            this.dispatcher.AddHandler(new User_SaveUser(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserImmediately(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserInfoToFile(this.server, this));
            this.dispatcher.AddHandler(new User_ServerKick(this.server, this));
            this.dispatcher.AddHandler(new User_SetGmFlag(this.server, this));
            this.dispatcher.AddHandler(new User_Shutdown(this.server, this));
            this.dispatcher.AddHandler(new User_Start(this.server, this));
            this.dispatcher.AddHandler(new User_UserDisconnectFromGateway(this.server, this));
            this.dispatcher.AddHandler(new User_UserLoginSuccess(this.server, this));
        }

        public override async Task Detach()
        {
            await base.Detach();
        }

        ServiceRuntimeInfo CreateRuntimeInfo()
        {
            var info = new ServiceRuntimeInfo();
            info.serviceId = this.serviceId;
            info.busyCount = this.sd.userCount;
            info.allowNew = this.sd.allowNewUser;
            return info;
        }

        public async Task UpdateRuntimeInfo()
        {
            await this.server.userServiceRuntimeInfoRedisW.Update(this.CreateRuntimeInfo());
        }

        public async Task CheckUpdateRuntimeInfo()
        {
            if (Math.Abs(this.sd.userCountDelta) >= 100)
            {
                this.sd.userCountDelta = 0;
                await this.UpdateRuntimeInfo();
            }
        }
    }
}