using Data;

namespace Script
{
    // 别人同意我的好友申请
    public class _User_OtherAcceptFriendRequest : Handler<UserService, MsgOtherAcceptFriendRequest, ResOtherAcceptFriendRequest>
    {
        public override MsgType msgType => MsgType._User_OtherAcceptFriendRequest;
        public _User_OtherAcceptFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgOtherAcceptFriendRequest msg, ResOtherAcceptFriendRequest res)
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

            bool blocked = user.userInfo.blockedUsers.Exists(x => x.userId == msg.otherUserId);
            if (blocked)
            {
                return ECode.Blocked;
            }


            int index = user.userInfo.outgoingFriendRequests.FindIndex(x => x.toUserId == msg.otherUserId);
            if (index < 0)
            {
                return ECode.OutgoingFriendRequestNotExist;
            }

            OutgoingFriendRequest req = user.userInfo.outgoingFriendRequests[index];
            if (req.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            //// ok

            req.result = FriendRequestResult.Accepted;

            bool alreadyFriends = user.userInfo.friends.Exists(x => x.userId == msg.otherUserId);
            if (!alreadyFriends)
            {
                var friendInfo = FriendInfo.Ensure(null);
                friendInfo.userId = msg.otherUserId;
                friendInfo.timeS = TimeUtils.GetTimeS();
                user.userInfo.friends.Add(friendInfo);
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgOtherAcceptFriendRequest msg, ECode e, ResOtherAcceptFriendRequest res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}