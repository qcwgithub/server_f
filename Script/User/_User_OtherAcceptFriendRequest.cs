using Data;

namespace Script
{
    // 别人同意我的好友申请
    [AutoRegister]
    public class _User_OtherAcceptFriendRequest : Handler<UserService, MsgOtherAcceptFriendRequest, ResOtherAcceptFriendRequest>
    {
        public override MsgType msgType => MsgType._User_OtherAcceptFriendRequest;
        public _User_OtherAcceptFriendRequest(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgOtherAcceptFriendRequest msg, ResOtherAcceptFriendRequest res)
        {
            string log = $"{this.msgType} otherUserId {msg.otherUserId}";
            this.service.logger.Info(log);
            MyDebug.Assert(msg.privateRoomId > 0);

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

            UserInfo userInfo = user!.userInfo;

            bool blocked = userInfo.blockedUsers.Exists(x => x.userId == msg.otherUserId);
            if (blocked)
            {
                return ECode.Blocked;
            }

            int index = userInfo.outgoingFriendRequests.FindIndex(x => x.toUserId == msg.otherUserId);
            if (index < 0)
            {
                return ECode.OutgoingFriendRequestNotExist;
            }

            OutgoingFriendRequest req = userInfo.outgoingFriendRequests[index];
            if (req.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            //// ok

            req.result = FriendRequestResult.Accepted;

            FriendInfo? removedFriendInfo = userInfo.removedFriends.Find(x => x.userId == msg.otherUserId);

            long readSeq = 0;
            long receivedSeq = 0;
            if (removedFriendInfo != null)
            {
                if (msg.privateRoomId == removedFriendInfo.roomId)
                {
                    readSeq = removedFriendInfo.readSeq;
                    receivedSeq = removedFriendInfo.receivedSeq;
                }
                else
                {
                    this.service.logger.Error($"{log} msg.privateRoomId {msg.privateRoomId} != removedFriendInfo.roomId {removedFriendInfo.roomId}, set readSeq = 0, receivedSeq = 0");
                }
            }
            FriendInfo friendInfo = this.service.friendScript.DoAddFriend(userInfo, msg.otherUserId, TimeUtils.GetTimeS(), msg.privateRoomId, readSeq, receivedSeq);

            if (user.connection != null)
            {
                user.connection.Send(MsgType.AOtherAcceptFriendRequest, new MsgAOtherAcceptFriendRequest
                {
                    otherUserId = msg.otherUserId,
                    friendInfo = friendInfo,
                }, null);
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