using Data;

namespace Script
{
    // 别人给我发送好友申请
    public class _User_ReceiveFriendRequest : Handler<UserService, MsgReceiveFriendRequest, ResReceiveFriendRequest>
    {
        public override MsgType msgType => MsgType._User_ReceiveFriendRequest;
        public _User_ReceiveFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReceiveFriendRequest msg, ResReceiveFriendRequest res)
        {
            this.service.logger.Info($"{this.msgType} fromUserId {msg.fromUserId}");

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

            bool blocked = user.userInfo.blockedUsers.Exists(x => x.userId == msg.fromUserId);
            if (blocked)
            {
                return ECode.Blocked;
            }

            //// ok

            int index = user.userInfo.incomingFriendRequests.FindIndex(x => x.fromUserId == msg.fromUserId);
            if (index >= 0)
            {
                user.userInfo.incomingFriendRequests.RemoveAt(index);
            }

            var req = IncomingFriendRequest.Ensure(null);
            req.fromUserId = msg.fromUserId;
            req.timeS = TimeUtils.GetTimeS();
            req.say = msg.say;
            req.result = FriendRequestResult.Wait;
            user.userInfo.incomingFriendRequests.Add(req);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReceiveFriendRequest msg, ECode e, ResReceiveFriendRequest res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}