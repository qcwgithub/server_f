using Data;

namespace Script
{
    [AutoRegister]
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

            UserInfo userInfo = user.userInfo;

            // bool alreadyFriends = userInfo.friends.Exists(x => x.userId == msg.fromUserId);
            // if (alreadyFriends)
            // {
            //     return ECode.AlreadyFriends;
            // }

            int incomingIndex = userInfo.incomingFriendRequests.FindIndex(x => x.fromUserId == msg.fromUserId);
            if (incomingIndex < 0)
            {
                return ECode.IncomingFriendRequestNotExist;
            }

            IncomingFriendRequest incomingReq = userInfo.incomingFriendRequests[incomingIndex];
            if (incomingReq.result == FriendRequestResult.Accepted)
            {
                FriendInfo? exist = userInfo.friends.Find(x => x.userId == msg.fromUserId);
                if (exist != null)
                {
                    res.friendInfo = exist;
                    return ECode.Success;
                }
            }
            else if (incomingReq.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            MyResponse r;

            FriendInfo? removedFriendInfo = userInfo.removedFriends.Find(x => x.userId == msg.fromUserId);

            long roomId = 0;
            long readSeq = 0;
            long receivedSeq = 0;
            if (removedFriendInfo != null)
            {
                roomId = removedFriendInfo.roomId;
                readSeq = removedFriendInfo.readSeq;
                receivedSeq = removedFriendInfo.receivedSeq;
            }
            else
            {
                var msgCreateRoom = new MsgRoomManagerCreateFriendChatRoom
                {
                    userIds = [user.userId, msg.fromUserId],
                };

                r = await this.service.roomManagerServiceProxy.CreateFriendChatRoom(msgCreateRoom);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                var resCreateRoom = r.CastRes<ResRoomManagerCreateFriendChatRoom>();
                MyDebug.Assert(resCreateRoom.friendChatRoomInfo.roomId > 0);
                roomId = resCreateRoom.friendChatRoomInfo.roomId;
            }
            MyDebug.Assert(roomId > 0);

            var msgOther = new MsgOtherAcceptFriendRequest
            {
                userId = msg.fromUserId,
                otherUserId = user.userId,
                roomId = roomId,
            };

            r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.fromUserId, MsgType._User_OtherAcceptFriendRequest, msgOther, true);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok

            incomingReq.result = FriendRequestResult.Accepted;

            FriendInfo friendInfo = this.service.friendScript.DoAddFriend(userInfo, msg.fromUserId, TimeUtils.GetTimeS(), roomId, readSeq, receivedSeq);

            res.friendInfo = friendInfo;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgAcceptFriendRequest msg, ECode e, ResAcceptFriendRequest res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}