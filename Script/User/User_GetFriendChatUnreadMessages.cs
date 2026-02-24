using Data;

namespace Script
{
    [AutoRegister]
    public class User_GetFriendChatUnreadMessages : Handler<UserService, MsgGetFriendChatUnreadMessages, ResGetFriendChatUnreadMessages>
    {
        public override MsgType msgType => MsgType.GetFriendChatUnreadMessages;
        public User_GetFriendChatUnreadMessages(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgGetFriendChatUnreadMessages msg, ResGetFriendChatUnreadMessages res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            UserFriendChatState state = await this.server.userFriendChatStateProxy.Get(this.service.dbServiceProxy, user.userId);
            if (state == null)
            {
                res.messages = [];
                res.hasMore = false;
                return ECode.Success;
            }

            MsgQuery_FriendChatMessages_by_roomId_readSeqs? msgDb = null;
            foreach (var kv in state.roomDict)
            {
                long roomId = kv.Key;
                UserFriendChatStateRoom stateRoom = kv.Value;
                MyDebug.Assert(stateRoom.unreadCount >= 0);
                if (stateRoom.unreadCount == 0)
                {
                    continue;
                }

                if (msgDb == null)
                {
                    msgDb = new MsgQuery_FriendChatMessages_by_roomId_readSeqs();
                    msgDb.roomIdToReadSeqs = new Dictionary<long, long>();
                }

                msgDb.roomIdToReadSeqs[roomId] = stateRoom.readSeq;
            }

            if (msgDb == null)
            {
                res.messages = [];
                res.hasMore = false;
                return ECode.Success;
            }

            var r = await this.service.dbServiceProxy.Query_FriendChatMessages_by_roomId_readSeqs(msgDb);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resDb = r.CastRes<ResQuery_FriendChatMessages_by_roomId_readSeqs>();
            res.messages = resDb.messages;
            res.hasMore = false;

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetFriendChatUnreadMessages msg, ECode e, ResGetFriendChatUnreadMessages res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}