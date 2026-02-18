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
        protected override ConnectionCallbackScript CreateConnectionCallbackScript()
        {
            return new UserConnectionCallbackScript(this.server, this);
        }

        public readonly UserServiceScript ss;
        public readonly ObjectLocator roomLocator;
        public readonly FriendScript friendScript;

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
            this.friendScript = new FriendScript(this.server, this);
        }

        public override void Attach()
        {
            base.Attach();
            base.AddHandler<UserService>();

            this.dispatcher.AddHandler(new User_AcceptFriendRequest(this.server, this));
            this.dispatcher.AddHandler(new _User_Action(this.server, this));
            this.dispatcher.AddHandler(new User_SendFriendRequest(this.server, this));
            this.dispatcher.AddHandler(new User_EnterRoom(this.server, this));
            this.dispatcher.AddHandler(new User_GetRecommendedRooms(this.server, this));
            this.dispatcher.AddHandler(new User_GetRoomChatHistory(this.server, this));
            this.dispatcher.AddHandler(new User_LeaveRoom(this.server, this));
            this.dispatcher.AddHandler(new _User_OnReloadConfigs(this.server, this), true);
            this.dispatcher.AddHandler(new _User_OnTimer(this.server, this), true);
            this.dispatcher.AddHandler(new User_RejectFriendRequest(this.server, this), true);
            this.dispatcher.AddHandler(new User_ReportRoomMessage(this.server, this));
            this.dispatcher.AddHandler(new User_ReportUser(this.server, this));
            this.dispatcher.AddHandler(new _User_ResetName(this.server, this));
            this.dispatcher.AddHandler(new _User_SaveUserImmediately(this.server, this));
            this.dispatcher.AddHandler(new _User_SaveUserInfoToFile(this.server, this));
            this.dispatcher.AddHandler(new User_SearchRoom(this.server, this));
            this.dispatcher.AddHandler(new User_SendRoomChat(this.server, this));
            this.dispatcher.AddHandler(new _User_ServerKick(this.server, this));
            this.dispatcher.AddHandler(new User_SetAvatarIndex(this.server, this));
            this.dispatcher.AddHandler(new _User_SetGmFlag(this.server, this));
            this.dispatcher.AddHandler(new User_SetName(this.server, this));
            this.dispatcher.AddHandler(new _User_UserDisconnectFromGateway(this.server, this));
            this.dispatcher.AddHandler(new _User_UserLoginSuccess(this.server, this));
            this.dispatcher.AddHandler(new User_BlockUser(this.server, this));
            this.dispatcher.AddHandler(new _User_ReceiveFriendRequest(this.server, this));
            this.dispatcher.AddHandler(new User_RemoveFriend(this.server, this));
            this.dispatcher.AddHandler(new _User_OtherRemoveFriend(this.server, this));
            this.dispatcher.AddHandler(new User_GetUserBriefInfos(this.server, this));
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
                return;
            }

            if (lockedUser.owner == owner)
            {
                lockedUser.owner = null;
                this.sd.lockedUserDict.Remove(userId);

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