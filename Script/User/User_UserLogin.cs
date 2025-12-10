using Data;

namespace Script
{
    public class User_UserLogin : UserHandler<MsgUserLogin, ResUserLogin>
    {
        public User_UserLogin(Server server, UserService service) : base(server, service)
        {
        }


        public override MsgType msgType { get { return MsgType.UserLogin; } }

        public static void HandleOldSocket(Service service, User user)
        {
            ProtocolClientData? oldSocket = user.socket;
            if (oldSocket == null)
            {
                return;
            }

            // 情况1 同一个客户端意外地登录2次
            // 情况2 客户端A已经登录，B再登录
            service.logger.InfoFormat("2 userId: {0}, ECode.OldSocket oldSocket: {1}, kick oldSocket.", user.userId, oldSocket.GetSocketId());

            service.tcpClientScript.UnbindUser(oldSocket, user);

            // 给客户端发消息...
            if (oldSocket.IsConnected())
            {
                var msgKick = new MsgKick();
                msgKick.flags = LogoutFlags.CancelAutoLogin;
                oldSocket.Send<MsgKick>(MsgType.Kick, msgKick);
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

        public override async Task<ECode> Handle(ProtocolClientData socket, MsgUserLogin msg, ResUserLogin res)
        {
            string message0 = string.Format("{0} userId {1} preCount {2}", this.msgType, msg.userId, this.usData.userDict.Count);

            if (this.service.data.state != ServiceState.Started)
            {
                this.logger.Info(message0 + ": server not ready, should go to AAA");
                return ECode.ServerNotReady;
            }

            long userId = msg.userId;

            if (userId <= 0 || msg.token == null)
            {
                this.logger.Info(message0 + ": userId <= 0 || msg.token == null, should go to AAA");
                return ECode.InvalidParam;
            }

            User? user = this.usData.GetUser(userId);
            if (user == null)
            {
                this.logger.Info(message0 + ": user == null, should go to AAA");
                return ECode.ShouldLoginAAA;
            }

            if (user.destroying)
            {
                // 其实不是错误，但是想要知道一下有没有触发这种情况
                this.logger.Error(message0 + ": user.destroying, should go to AAA");
                return ECode.ShouldLoginAAA;
            }

            if (!user.IsRealPrepareLogin(out MsgPrepareUserLogin msgPrepare))
            {
                this.logger.Error(message0 + ": !IsRealPrepareLogin, should go to AAA");
                return ECode.ShouldLoginAAA;
            }

            if (msg.token != msgPrepare.token)
            {
                this.logger.Error(message0 + ": msg.token != msgPreparePlayerLogin.token, should go to AAA");
                return ECode.InvalidToken;
            }

            int delayS = this.NeedDelayLogin(user);
            if (delayS > 0)
            {
                this.logger.Info(message0 + ": delayS > 0, should wait");
                res.delayS = delayS;
                return ECode.DelayLogin;
            }

            // 除了 PMPreparePlayerLogin，这里也需要对 oldSocket 做检测，因为客户端重连时不会经过 PMPreparePlayerLogin
            var oldSocket = user.socket;
            bool kickOther = false;
            if (user.socket != null)
            {
                kickOther = true;
                HandleOldSocket(this.service, user);
            }

            var oldUser = this.GetUser(socket);
            if (oldUser != null)
            {
                // 这个分支都没走过

                // 情况1 同一个客户端意外地登录2次
                // 情况2 客户端A已经登录，B再登录
                this.logger.Error(message0 + $": oldUser != null ({oldUser.userId}), should go to AAA");
                return ECode.OldUser;
            }

            this.logger.Info(message0 + ": check ok");

            if (user.destroyTimer.IsAlive())
            {
                this.usScript.ClearDestroyTimer(user, true);
            }

            this.service.tcpClientScript.BindUser(socket, user);

            long nowS = TimeUtils.GetTimeS();
            user.onlineTimeS = nowS;
            if (user.onlineTimeS <= user.offlineTimeS)
            {
                user.onlineTimeS = user.offlineTimeS + 1;
            }

            // 发送玩家数据
            // int aaaId=
            // var locAAA = this.server.GetKnownLoc(ServerConst.AAA_ID);
            res.userId = userId;
            res.profile = user.profile;
            res.kickOther = kickOther;
            user.profile.lastLoginTimeS = nowS;
            return ECode.Success;
        }
    }
}