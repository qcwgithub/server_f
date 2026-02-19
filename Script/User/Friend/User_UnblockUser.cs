using Data;

namespace Script
{
    [AutoRegister]
    public class User_UnblockUser : Handler<UserService, MsgUnblockUser, ResUnblockUser>
    {
        public override MsgType msgType => MsgType.UnblockUser;
        public User_UnblockUser(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgUnblockUser msg, ResUnblockUser res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} targetUserId {msg.targetUserId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            int index = user.userInfo.blockedUsers.FindIndex(x => x.userId == msg.targetUserId);
            if (index < 0)
            {
                return ECode.Success;
            }

            user.userInfo.blockedUsers.RemoveAt(index);
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgUnblockUser msg, ECode e, ResUnblockUser res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}