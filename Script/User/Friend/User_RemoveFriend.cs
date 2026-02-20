using Data;

namespace Script
{
    [AutoRegister]
    public class User_RemoveFriend : Handler<UserService, MsgRemoveFriend, ResRemoveFriend>
    {
        public override MsgType msgType => MsgType.RemoveFriend;
        public User_RemoveFriend(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgRemoveFriend msg, ResRemoveFriend res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} friendUserId {msg.friendUserId}");

            if (msg.friendUserId <= 0)
            {
                return ECode.InvalidUserId;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.friendUserId == user.userId)
            {
                return ECode.InvalidParam;
            }

            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.friendUserId);
            if (friendIndex < 0)
            {
                return ECode.NotFriends;
            }

            var r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.friendUserId, MsgType._User_OtherRemoveFriend, new MsgOtherRemoveFriend
            {
                userId = msg.friendUserId,
                otherUserId = user.userId
            }, true);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok
            this.service.friendScript.DoRemoveFriend(userInfo, msg.friendUserId, friendIndex, TimeUtils.GetTimeS());
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRemoveFriend msg, ECode e, ResRemoveFriend res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}