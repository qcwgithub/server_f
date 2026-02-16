using Data;

namespace Script
{
    public class User_AcceptFriendRequest : Handler<UserService, MsgAcceptFriendRequest, ResAcceptFriendRequest>
    {
        public override MsgType msgType => MsgType.AcceptFriendRequest;
        public User_AcceptFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgAcceptFriendRequest msg, ResAcceptFriendRequest res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} fromUserId {msg.fromUserId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            bool alreadyFriends = user.userInfo.friends.Exists(x => x.userId == msg.fromUserId);
            if (alreadyFriends)
            {
                return ECode.AlreadyFriends;
            }

            int index = user.userInfo.incomingFriendRequests.FindIndex(x => x.fromUserId == msg.fromUserId);
            if (index < 0)
            {
                return ECode.IncomingFriendRequestNotExist;
            }

            IncomingFriendRequest req = user.userInfo.incomingFriendRequests[index];
            if (req.result == FriendRequestResult.Accepted)
            {
                return ECode.Success;
            }
            if (req.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            var r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.fromUserId, MsgType._User_OtherAcceptFriendRequest, new MsgOtherAcceptFriendRequest
            {
                userId = msg.fromUserId,
                otherUserId = user.userId,
            });
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok

            req.result = FriendRequestResult.Accepted;

            var friendInfo = FriendInfo.Ensure(null);
            friendInfo.userId = msg.fromUserId;
            friendInfo.timeS = TimeUtils.GetTimeS();
            user.userInfo.friends.Add(friendInfo);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgAcceptFriendRequest msg, ECode e, ResAcceptFriendRequest res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}