using Data;

namespace Script
{
    [AutoRegister]
    public class User_AckFriendChatReadSeqN : Handler<UserService, MsgAckFriendChatReadSeqN, ResAckFriendChatReadSeqN>
    {
        public override MsgType msgType => MsgType.AckFriendChatReadSeqN;
        public User_AckFriendChatReadSeqN(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgAckFriendChatReadSeqN msg, ResAckFriendChatReadSeqN res)
        {
            string log = $"{this.msgType} userId {context.msg_userId} dict {JsonUtils.stringify(msg.roomIdToReadSeqs)}";
            this.service.logger.Info(log);

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            UserFriendChatState state = await this.server.userFriendChatStateProxy.Get(this.service.dbServiceProxy, user.userId);
            if (state == null)
            {
                this.service.logger.Error($"{log} state == null");
                return ECode.Success;
            }

            var dict = new Dictionary<long, (long, long?)>();
            foreach (var kv in msg.roomIdToReadSeqs)
            {
                long roomId = kv.Key;
                long readSeq = kv.Value;

                if (!state.roomDict.TryGetValue(roomId, out UserFriendChatStateRoom? stateRoom))
                {
                    this.service.logger.Error($"{log} roomId {roomId} stateRoom == null");
                    continue;
                }

                if (readSeq > stateRoom.maxSeq)
                {
                    this.service.logger.Error($"{log} roomId {roomId} readSeq > stateRoom.maxSeq {stateRoom.maxSeq}");
                    continue;
                }

                if (readSeq == stateRoom.maxSeq && stateRoom.unreadCount == 0)
                {
                    continue;
                }

                dict[roomId] = (readSeq,
                    readSeq == stateRoom.maxSeq && stateRoom.unreadCount > 0
                        ? stateRoom.unreadCount
                        : null);
            }

            if (dict.Count > 0)
            {
                await this.server.userFriendChatStateProxy.SetReadSeq_DecreaseUnreadCount_N(user.userId, dict);
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgAckFriendChatReadSeqN msg, ECode e, ResAckFriendChatReadSeqN res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}