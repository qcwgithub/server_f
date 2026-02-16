using Data;

namespace Script
{
    public class User_RejectFriendRequest : Handler<UserService, MsgRejectFriendRequest, ResRejectFriendRequest>
    {
        public override MsgType msgType => MsgType.RejectFriendRequest;
        public User_RejectFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgRejectFriendRequest msg, ResRejectFriendRequest res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} fromUserId {msg.fromUserId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            int index = user.userInfo.incomingFriendRequests.FindIndex(x => x.fromUserId == msg.fromUserId);
            if (index < 0)
            {
                return ECode.IncomingFriendRequestNotExist;
            }

            IncomingFriendRequest req = user.userInfo.incomingFriendRequests[index];
            if (req.result == FriendRequestResult.Rejected)
            {
                return ECode.Success;
            }
            if (req.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            var r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.fromUserId, MsgType._User_OtherRejectFriendRequest, new MsgOtherRejectFriendRequest
            {
                userId = msg.fromUserId,
                otherUserId = user.userId,
            });
            if (r.e != ECode.Success)
            {
                if (r.e == ECode.OutgoingFriendRequestNotExist ||
                    r.e == ECode.FriendRequestResultNotWait)
                {

                }
                else
                {

                    return r.e;
                }
            }

            /// ok

            req.result = FriendRequestResult.Rejected;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRejectFriendRequest msg, ECode e, ResRejectFriendRequest res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}