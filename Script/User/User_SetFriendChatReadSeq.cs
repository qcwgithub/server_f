using Data;

namespace Script
{
    [AutoRegister]
    public class User_SetFriendChatReadSeq : Handler<UserService, MsgSetFriendChatReadSeq, ResSetFriendChatReadSeq>
    {
        public override MsgType msgType => MsgType.SetFriendChatReadSeq;
        public User_SetFriendChatReadSeq(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSetFriendChatReadSeq msg, ResSetFriendChatReadSeq res)
        {
            string log = $"{this.msgType} userId {context.msg_userId} friendUserId {msg.friendUserId} readSeq {msg.readSeq}";
            this.service.logger.Info(log);

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.friendUserId);
            if (friendIndex < 0)
            {
                return ECode.NotFriends;
            }

            FriendInfo friendInfo = userInfo.friends[friendIndex];
            if (msg.readSeq < friendInfo.readSeq)
            {
                this.service.logger.Error($"{log} msg.readSeq < friendInfo.readSeq {friendInfo.readSeq}");
                return ECode.Error;
            }

            //// ok

            friendInfo.readSeq = msg.readSeq;

            res.readSeq = friendInfo.readSeq;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSetFriendChatReadSeq msg, ECode e, ResSetFriendChatReadSeq res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}