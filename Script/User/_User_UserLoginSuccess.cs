using Data;

namespace Script
{
    [AutoRegister]
    public class _User_UserLoginSuccess : Handler<UserService, MsgUserLoginSuccess, ResUserLoginSuccess>
    {
        public _User_UserLoginSuccess(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType { get { return MsgType._User_UserLoginSuccess; } }

        public override async Task<ECode> Handle(MessageContext context, MsgUserLoginSuccess msg, ResUserLoginSuccess res)
        {
            string message0 = string.Format("{0} userId {1} preCount {2}", this.msgType, msg.userId, this.service.sd.userCount);

            if (this.service.data.state != ServiceState.Started)
            {
                this.service.logger.Info(message0 + ": server not ready");
                return ECode.ServerNotReady;
            }

            ECode e;
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                UserInfo? userInfo = null;
                if (msg.isNewUser)
                {
                    userInfo = msg.newUserInfo;
                    if (userInfo == null)
                    {
                        this.service.logger.Error($"{this.msgType} msg.isNewUser but newUserInfo is null");
                        return ECode.UserInfoNotExist;
                    }
                }

                (e, user) = await this.service.LoadUser(msg.userId, userInfo);
                if (e != ECode.Success)
                {
                    return e;
                }

                if (user == null)
                {
                    return ECode.UserNotExist;
                }
            }

            this.service.ss.SetSaveTimer(user);

            bool kickOther = this.HandleOldConnection(user, msg.gatewayServiceId);

            if (user.connection == null)
            {
                user.connection = new UserConnection(msg.gatewayServiceId, user, this.service.sd);
            }

            this.service.logger.Info(message0 + ": check ok");

            this.service.ss.ClearDestroyTimer(user, UserClearDestroyTimerReason.UserLoginSuccess);

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

        bool HandleOldConnection(User user, int gatewayServiceId)
        {
            if (user.connection == null || user.connection.gatewayServiceId == gatewayServiceId)
            {
                return false;
            }

            UserConnection oldConnection = user.connection;

            // 情况1 同一个客户端意外地登录2次
            // 情况2 客户端A已经登录，B再登录
            this.service.logger.InfoFormat("userId {0} gatewayServiceId {1} old {2}, kick old",
                user.userId, gatewayServiceId, oldConnection.gatewayServiceId);

            user.connection = null;

            // 给客户端发消息...
            if (oldConnection.IsConnected())
            {
                var msgKick = new MsgKick();
                msgKick.flags = LogoutFlags.CancelAutoLogin;
                oldConnection.Send(MsgType.Kick, msgKick, null);
            }

            return true;
        }

        public override void PostHandle(MessageContext context, MsgUserLoginSuccess msg, ECode e, ResUserLoginSuccess res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}