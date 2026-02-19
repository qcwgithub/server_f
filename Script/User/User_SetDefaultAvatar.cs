using Data;

namespace Script
{
    [AutoRegister]
    public class User_SetAvatarIndex : Handler<UserService, MsgSetAvatarIndex, ResSetAvatarIndex>
    {
        public override MsgType msgType => MsgType.SetAvatarIndex;
        public User_SetAvatarIndex(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSetAvatarIndex msg, ResSetAvatarIndex res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} avatarIndex {msg.avatarIndex}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            var userAvatarConfig = this.server.data.serverConfig.userAvatarConfig;

            // check time
            long nowS = TimeUtils.GetTimeS();
            if (nowS - user.userInfo.lastSetAvatarIndexTimeS < userAvatarConfig.minIntervalS)
            {
                return ECode.AvatarIndex_TooFrequent;
            }

            if (msg.avatarIndex < userAvatarConfig.minIndex ||
                msg.avatarIndex > userAvatarConfig.maxIndex)
            {
                return ECode.AvatarIndex_OutOfRange;
            }

            //// ok

            user.userInfo.lastSetAvatarIndexTimeS = nowS;
            user.userInfo.avatarIndex = msg.avatarIndex;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSetAvatarIndex msg, ECode e, ResSetAvatarIndex res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}