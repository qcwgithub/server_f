using Data;

namespace Script
{
    public class User_SetDefaultAvatar : Handler<UserService, MsgSetDefaultAvatar, ResSetDefaultAvatar>
    {
        public override MsgType msgType => MsgType.SetDefaultAvatar;
        public User_SetDefaultAvatar(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSetDefaultAvatar msg, ResSetDefaultAvatar res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} defaultAvatarIndex {msg.defaultAvatarIndex}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var userDefaultAvatarConfig = this.server.data.serverConfig.userDefaultAvatarConfig;

            // check time
            long nowS = TimeUtils.GetTimeS();
            if (nowS - user.userInfo.lastSetDefaultAvatarTimeS < userDefaultAvatarConfig.minIntervalS)
            {
                return ECode.DefaultAvatar_TooFrequent;
            }

            if (msg.defaultAvatarIndex < userDefaultAvatarConfig.minIndex ||
                msg.defaultAvatarIndex > userDefaultAvatarConfig.maxIndex)
            {
                return ECode.DefaultAvatar_OutofRange;
            }

            //// ok

            user.userInfo.lastSetDefaultAvatarTimeS = nowS;
            user.userInfo.defaultAvatarIndex = msg.defaultAvatarIndex;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSetDefaultAvatar msg, ECode e, ResSetDefaultAvatar res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}