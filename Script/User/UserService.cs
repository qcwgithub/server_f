using Data;

namespace Script
{
    public partial class UserService : Service
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
        public readonly UserManagerServiceProxy userManagerServiceProxy;

        protected override MessageDispatcher CreateMessageDispatcher()
        {
            return new UserMessageDispatcher(this.server, this);
        }
        protected override ProtocolClientScriptForS CreateProtocolClientScriptForS()
        {
            return new UserProtocolClientScriptForS(this.server, this);
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
            this.AddServiceProxy(this.userManagerServiceProxy = new UserManagerServiceProxy(this));

            this.ss = new UserServiceScript(this.server, this);

            this.roomLocator = ObjectLocator.CreateRoomLocator(this.server, this, this.sd.roomLocatorData);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserService>();

            this.dispatcher.AddHandler(new User_Action(this.server, this));
            this.dispatcher.AddHandler(new User_EnterRoom(this.server, this));
            this.dispatcher.AddHandler(new User_LeaveRoom(this.server, this));
            this.dispatcher.AddHandler(new User_OnReloadConfigs(this.server, this), true);
            this.dispatcher.AddHandler(new User_OnTimer(this.server, this), true);
            this.dispatcher.AddHandler(new User_SaveUserImmediately(this.server, this));
            this.dispatcher.AddHandler(new User_SaveUserInfoToFile(this.server, this));
            this.dispatcher.AddHandler(new User_ServerKick(this.server, this));
            this.dispatcher.AddHandler(new User_SetGmFlag(this.server, this));
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

        public async Task<User?> LockUser(long userId, object owner)
        {
            MyDebug.Assert(owner != null);

            User? user = this.sd.GetUser(userId);

            if (!this.sd.lockedUserDict.TryGetValue(userId, out var lockedUser))
            {
                this.sd.lockedUserDict[userId] = new UserServiceData.LockedUser
                {
                    owner = owner,
                };
                return user;
            }

            if (lockedUser.owner == null)
            {
                lockedUser.owner = owner;
                return user;
            }

            if (lockedUser.owner == owner)
            {
                return user;
            }

            if (lockedUser.waiting == null)
            {
                lockedUser.waiting = new List<TaskCompletionSource>();
            }

            var tcs = new TaskCompletionSource();
            lockedUser.waiting.Add(tcs);
            await tcs.Task;

            MyDebug.Assert(lockedUser.owner == null);
            lockedUser.owner = owner;
            return user;
        }

        public void TryUnlockUser(long userId, object owner)
        {
            MyDebug.Assert(owner != null);

            if (!this.sd.lockedUserDict.TryGetValue(userId, out var lockedUser))
            {
                MyDebug.Assert(false);
                return;
            }

            if (lockedUser.owner == owner)
            {
                lockedUser.owner = null;

                if (lockedUser.waiting != null && lockedUser.waiting.Count > 0)
                {
                    var tcs = lockedUser.waiting[0];
                    lockedUser.waiting.RemoveAt(0);
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