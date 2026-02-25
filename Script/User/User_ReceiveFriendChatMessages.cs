using Data;

namespace Script
{
    [AutoRegister]
    public class User_ReceiveFriendChatMessages : Handler<UserService, MsgReceiveFriendChatMessages, ResReceiveFriendChatMessages>
    {
        public override MsgType msgType => MsgType.ReceiveFriendChatMessages;
        public User_ReceiveFriendChatMessages(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReceiveFriendChatMessages msg, ResReceiveFriendChatMessages res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            UserInfo userInfo = user.userInfo;
            if (userInfo.friends.Count == 0)
            {
                res.messages = [];
                res.hasMore = false;
                return ECode.Success;
            }

            res.messages = [];
            res.hasMore = false;

            Task<ChatMessage[]>[] tasks = userInfo.friends.Select(x => this.server.friendChatMessagesRedis.GetAll(x.roomId)).ToArray();
            await Task.WhenAll(tasks);

            var msgDb = new MsgQuery_FriendChatMessages_by_roomId_receivedSeqs();
            msgDb.roomIdToReceivedSeqs = new Dictionary<long, long>();
            foreach (FriendInfo friendInfo in userInfo.friends)
            {
                msgDb.roomIdToReceivedSeqs[friendInfo.roomId] = friendInfo.receivedSeq;
            }

            var r = await this.service.dbServiceProxy.Query_FriendChatMessages_by_roomId_receivedSeqs(msgDb);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResQuery_FriendChatMessages_by_roomId_receivedSeqs>();
            res.messages = resDb.messages;
            res.hasMore = false;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReceiveFriendChatMessages msg, ECode e, ResReceiveFriendChatMessages res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}