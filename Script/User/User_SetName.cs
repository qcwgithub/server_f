using Data;

namespace Script
{
    public class User_SetName : Handler<UserService, MsgSetName, ResSetName>
    {
        public override MsgType msgType => MsgType.SetName;
        public User_SetName(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSetName msg, ResSetName res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} userName {msg.userName}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var userNameConfig = this.server.data.serverConfig.userNameConfig;

            // check time
            long nowS = TimeUtils.GetTimeS();
            if (nowS - user.userInfo.lastSetNameTimeS < userNameConfig.minIntervalS)
            {
                return ECode.Name_TooFrequent;
            }

            // check userName
            if (msg.userName == null)
            {
                return ECode.Name_Empty;
            }

            msg.userName = msg.userName.Trim();

            if (msg.userName.Length < userNameConfig.minLength)
            {
                return ECode.Name_TooShort;
            }
            if (msg.userName.Length > userNameConfig.maxLength)
            {
                return ECode.Name_TooLong;
            }

            string lower = msg.userName.ToLower();
            if (lower.Contains("system") ||
                lower.Contains("moderator") ||
                lower.Contains("official") ||
                lower.Contains("admin") ||
                lower.Contains("http") ||
                lower.Contains("@") ||
                lower.Contains("www."))
            {
                return ECode.Name_Reserved;
            }

            //// ok

            user.userInfo.lastSetNameTimeS = nowS;
            user.userInfo.userName = msg.userName;

            res.userName = msg.userName;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSetName msg, ECode e, ResSetName res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}