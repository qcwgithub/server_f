using Data;

namespace Script
{
    [AutoRegister]
    public class User_BlockUser : Handler<UserService, MsgBlockUser, ResBlockUser>
    {
        public override MsgType msgType => MsgType.BlockUser;
        public User_BlockUser(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgBlockUser msg, ResBlockUser res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} targetUserId {msg.targetUserId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            bool blocked = user.userInfo.blockedUsers.Exists(x => x.userId == msg.targetUserId);
            if (blocked)
            {
                return ECode.Success;
            }

            var blockedUser = BlockedUser.Ensure(null);
            blockedUser.userId = msg.targetUserId;
            blockedUser.timeS = TimeUtils.GetTimeS();
            user.userInfo.blockedUsers.Add(blockedUser);
            
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgBlockUser msg, ECode e, ResBlockUser res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}