using Data;

namespace Script
{
    // 别人拒绝我的好友申请
    public class _User_RejectFriendRequest : Handler<UserService, MsgOtherRejectFriendRequest, ResOtherRejectFriendRequest>
    {
        public override MsgType msgType => MsgType._User_OtherRejectFriendRequest;
        public _User_RejectFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgOtherRejectFriendRequest msg, ResOtherRejectFriendRequest res)
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

            req.result = FriendRequestResult.Rejected;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgOtherRejectFriendRequest msg, ECode e, ResOtherRejectFriendRequest res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}