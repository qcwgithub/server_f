using Data;

namespace Script
{
    [AutoRegister]
    public class User_SetFriendChatReceivedSeq : Handler<UserService, MsgSetFriendChatReceivedSeq, ResSetFriendChatReceivedSeq>
    {
        public override MsgType msgType => MsgType.SetFriendChatReceivedSeq;
        public User_SetFriendChatReceivedSeq(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSetFriendChatReceivedSeq msg, ResSetFriendChatReceivedSeq res)
        {
            string log = $"{this.msgType} userId {context.msg_userId} friendUserId {msg.friendUserId} receivedSeq {msg.receivedSeq}";
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
            if (msg.receivedSeq < friendInfo.receivedSeq)
            {
                this.service.logger.Error($"{log} msg.receivedSeq < friendInfo.receivedSeq {friendInfo.receivedSeq}");
                return ECode.Error;
            }

            //// ok

            friendInfo.receivedSeq = msg.receivedSeq;

            res.receivedSeq = friendInfo.receivedSeq;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSetFriendChatReceivedSeq msg, ECode e, ResSetFriendChatReceivedSeq res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}