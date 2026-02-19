using Data;

namespace Script
{
    // 别人删除我
    [AutoRegister]
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
                (e, user) = await this.service.ss.SimulateUserLogin(msg.userId, UserDestroyUserReason.SimulateLogin);
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            UserInfo userInfo = user.userInfo;

            int friendIndex = userInfo.friends.FindIndex(x => x.userId == msg.otherUserId);
            if (friendIndex < 0)
            {
                return ECode.Success; // !
            }

            //// ok

            FriendInfo removedFriendInfo = this.service.friendScript.DoRemoveFriend(userInfo, msg.otherUserId, friendIndex, TimeUtils.GetTimeS());

            if (user.connection != null)
            {
                var broadcast = new MsgARemoveFriend
                {
                    friendUserId = msg.otherUserId,
                    reason = RemoveFriendReason.OtherRemoveYou,
                    removedFriendInfo = removedFriendInfo,
                };
                user.connection.Send(MsgType.ARemoveFriend, broadcast, null);
            }
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgOtherRemoveFriend msg, ECode e, ResOtherRemoveFriend res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}