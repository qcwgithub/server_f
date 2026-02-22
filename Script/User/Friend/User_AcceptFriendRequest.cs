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

            bool alreadyFriends = userInfo.friends.Exists(x => x.userId == msg.fromUserId);
            if (alreadyFriends)
            {
                return ECode.AlreadyFriends;
            }

            int index = userInfo.incomingFriendRequests.FindIndex(x => x.fromUserId == msg.fromUserId);
            if (index < 0)
            {
                return ECode.IncomingFriendRequestNotExist;
            }

            IncomingFriendRequest req = userInfo.incomingFriendRequests[index];
            if (req.result == FriendRequestResult.Accepted)
            {
                return ECode.Success;
            }
            if (req.result != FriendRequestResult.Wait)
            {
                return ECode.FriendRequestResultNotWait;
            }

            MyResponse r;

            int removedFriendIndex = userInfo.removedFriends.FindIndex(x => x.userId == msg.fromUserId);

            long privateRoomId = 0;
            if (removedFriendIndex >= 0)
            {
                privateRoomId = userInfo.removedFriends[removedFriendIndex].roomId;
            }
            else
            {
                var msgCreateRoom = new MsgRoomManagerCreatePrivateRoom
                {
                    participants = [user.userId, msg.fromUserId],
                };

                r = await this.service.roomManagerServiceProxy.CreatePrivateRoom(msgCreateRoom);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                var resCreateRoom = r.CastRes<ResRoomManagerCreatePrivateRoom>();
                MyDebug.Assert(resCreateRoom.friendChatInfo.roomId > 0);
                privateRoomId = resCreateRoom.friendChatInfo.roomId;
            }
            MyDebug.Assert(privateRoomId > 0);

            var msgOther = new MsgOtherAcceptFriendRequest
            {
                userId = msg.fromUserId,
                otherUserId = user.userId,
                privateRoomId = privateRoomId,
            };

            r = await this.service.userManagerServiceProxy.ForwardToUserService(msg.fromUserId, MsgType._User_OtherAcceptFriendRequest, msgOther, true);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            //// ok

            req.result = FriendRequestResult.Accepted;

            this.service.friendScript.DoAddFriend(userInfo, msg.fromUserId, TimeUtils.GetTimeS(), privateRoomId);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgAcceptFriendRequest msg, ECode e, ResAcceptFriendRequest res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}