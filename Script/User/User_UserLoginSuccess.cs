using Data;

namespace Script
{
    // <- Gateway
    public class User_UserLoginSuccess : UserHandler<MsgUserLoginSuccess, ResUserLoginSuccess>
    {
        public User_UserLoginSuccess(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType { get { return MsgType._User_UserLoginSuccess; } }

        public override async Task<ECode> Handle(IConnection connection, MsgUserLoginSuccess msg, ResUserLoginSuccess res)
        {
            // Gateway
            var serviceConnection = (ServiceConnection)connection;
            MyDebug.Assert(serviceConnection.serviceTypeAndId.Value.serviceType == ServiceType.Gateway);

            string message0 = string.Format("{0} userId {1} preCount {2}", this.msgType, msg.userId, this.sd.userDict.Count);

            if (this.service.data.state != ServiceState.Started)
            {
                this.logger.Info(message0 + ": server not ready");
                return ECode.ServerNotReady;
            }

            User? user = this.sd.GetUser(msg.userId);
            if (user != null)
            {
                if (user.destroying)
                {
                    // 其实不是错误，但是想要知道一下有没有触发这种情况
                    this.service.logger.ErrorFormat("{0} userId {1} destroying", this.msgType, user.userId);
                    return ECode.UserDestroying;
                }
            }
            else
            {
                UserInfo? userInfo;
                if (msg.isNewUser)
                {
                    userInfo = msg.newUserInfo;
                    MyDebug.Assert(userInfo != null);
                }
                else
                {
                    ECode e;
                    (e, userInfo) = await this.service.ss.QueryUserInfo(msg.userId);
                    if (e != ECode.Success)
                    {
                        return e;
                    }

                    if (userInfo == null)
                    {
                        return ECode.UserInfoNotExist;
                    }
                
                    user = new User(userInfo);
                    this.AddUserToDict(user);

                    // 这里不再加东西了，要加得加到 AddPlayerToDict 里
                }
            }

            if (!user.saveTimer.IsAlive())
            {
                this.service.ss.SetSaveTimer(user);
            }

            int delayS = this.NeedDelayLogin(user);
            if (delayS > 0)
            {
                this.logger.Info(message0 + ": delayS > 0, should wait");
                res.delayS = delayS;
                return ECode.DelayLogin;
            }

            bool kickOther = false;
            if (user.connection != null)
            {
                kickOther = true;
                this.HandleOldConnection(this.service, user);
            }

            User? oldUser = serviceConnection.GetUser();
            if (oldUser != null)
            {
                // 这个分支都没走过

                // 情况1 同一个客户端意外地登录2次
                // 情况2 客户端A已经登录，B再登录
                this.logger.Error(message0 + $": oldUser != null ({oldUser.userId})");
                return ECode.OldUser;
            }

            this.logger.Info(message0 + ": check ok");

            if (user.destroyTimer.IsAlive())
            {
                this.usScript.ClearDestroyTimer(user, true);
            }

            user.connection = serviceConnection;
            serviceConnection.BindUser(user);

            long nowS = TimeUtils.GetTimeS();
            user.onlineTimeS = nowS;
            if (user.onlineTimeS <= user.offlineTimeS)
            {
                user.onlineTimeS = user.offlineTimeS + 1;
            }

            res.userInfo = user.userInfo;
            res.kickOther = kickOther;

            user.userInfo.lastLoginTimeS = nowS;
            return ECode.Success;
        }

        void HandleOldConnection(Service service, User user)
        {
            UserConnection? oldConnection = user.connection;
            if (oldConnection == null)
            {
                return;
            }

            // 情况1 同一个客户端意外地登录2次
            // 情况2 客户端A已经登录，B再登录
            service.logger.InfoFormat("2 userId: {0}, ECode.OldConnection oldConnectionId: {1}, kick oldConnection.", user.userId, oldConnection.GetConnectionId());

            user.connection = null;
            oldConnection.UnbindUser();

            // 给客户端发消息...
            if (oldConnection.IsConnected())
            {
                var msgKick = new MsgKick();
                msgKick.flags = LogoutFlags.CancelAutoLogin;
                oldConnection.Send<MsgKick>(MsgType.Kick, msgKick);
            }
        }

        int NeedDelayLogin(User user)
        {
            if (user.processingMsgs.Count == 0)
            {
                user.accumDelayLoginS = 0;
                return 0;
            }

            if (user.accumDelayLoginS >= 10)
            {
                user.accumDelayLoginS = 0;
                return 0;
            }

            bool need = user.processingMsgs.Any(kv => kv.Key.DelayLoginIfHandling());
            if (need)
            {
                const int D = 1;
                user.accumDelayLoginS += D;
                this.service.logger.ErrorFormat("{0} userId {1} delay login, accumDelayLoginS {2}", this.msgType, user.userId, user.accumDelayLoginS);
                return D;
            }
            else
            {
                user.accumDelayLoginS = 0;
                return 0;
            }
        }

        void AddUserToDict(User user)
        {
            // runtime 初始化
            this.service.sd.userDict.Add(user.userId, user);

            // 有值就不能再赋值了，不然玩家上线下线就错了
            MyDebug.Assert(user.lastUserInfo == null);

            user.lastUserInfo = UserInfo.Ensure(null);
            user.lastUserInfo.DeepCopyFrom(user.userInfo);

            // qiucw
            // 这句会修改 userInfo，必须放在 lastUserInfo.DeepCopyFrom 后面
            // this.gameScripts.CallInit(user);
        }
    }
}