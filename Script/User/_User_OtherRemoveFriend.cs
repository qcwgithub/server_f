using Data;

namespace Script
{
    // 别人删除我
    public class _User_OtherRemoveFriend : Handler<UserService, MsgOtherRemoveFriend, ResOtherRemoveFriend>
    {
        public override MsgType msgType => MsgType._User_OtherRemoveFriend;
        public _User_OtherRemoveFriend(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgOtherRemoveFriend msg, ResOtherRemoveFriend res)
        {
            this.service.logger.Info($"{this.msgType} otherUserId {msg.otherUserId}");

            ECode e;
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                (e, user) = await this.service.ss.SimulateUserLogin(msg.userId, UserDestroyUserReason.SimulateLogin_ReveiveFriendRequest);
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            int index = user.userInfo.friends.FindIndex(x => x.userId == msg.otherUserId);
            if (index < 0)
            {
                return ECode.Success; // !
            }

            //// ok

            user.userInfo.friends.RemoveAt(index);
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgOtherRemoveFriend msg, ECode e, ResOtherRemoveFriend res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}