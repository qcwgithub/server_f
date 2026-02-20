using Data;

namespace Script
{
    [AutoRegister]
    public class User_SendFriendRequest : Handler<UserService, MsgSendFriendRequest, ResSendFriendRequest>
    {
        public override MsgType msgType => MsgType.SendFriendRequest;
        public User_SendFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSendFriendRequest msg, ResSendFriendRequest res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} toUserId {msg.toUserId}");

            if (msg.toUserId <= 0)
            {
                return ECode.InvalidUserId;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.toUserId == user.userId)
            {
                return ECode.InvalidParam;
            }

            bool alreadyFriends = user.userInfo.friends.Exists(x => x.userId == msg.toUserId);
            if (alreadyFriends)
            {
                return ECode.AlreadyFriends;
            }

            int existIndex = user.userInfo.outgoingFriendRequests.FindIndex(o => o.toUserId == msg.toUserId);
            if (existIndex >= 0 && user.userInfo.outgoingFriendRequests[existIndex].result != FriendRequestResult.Wait)
            {
                return ECode.OutgoingFriendRequestAlreadyExist;
            }
            var r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.toUserId, MsgType._User_ReceiveFriendRequest, new MsgReceiveFriendRequest
            {
                userId = msg.toUserId,
                fromUserId = user.userId,
                say = msg.say,
                fromUserBriefInfo = UserServiceScript.CreateUserBriefInfo(user.userInfo),
            }, true);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok

            if (existIndex >= 0)
            {
                user.userInfo.outgoingFriendRequests.RemoveAt(existIndex);
            }

            var req = OutgoingFriendRequest.Ensure(null);
            req.toUserId = msg.toUserId;
            req.timeS = TimeUtils.GetTimeS();
            req.say = msg.say;
            req.result = FriendRequestResult.Wait;
            user.userInfo.outgoingFriendRequests.Add(req);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgSendFriendRequest msg, ECode e, ResSendFriendRequest res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}