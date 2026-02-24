using Data;

namespace Script
{
    [AutoRegister]
    public class User_AckFriendChatReadSeq1 : Handler<UserService, MsgAckFriendChatReadSeq1, ResAckFriendChatReadSeq1>
    {
        public override MsgType msgType => MsgType.AckFriendChatReadSeq1;
        public User_AckFriendChatReadSeq1(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgAckFriendChatReadSeq1 msg, ResAckFriendChatReadSeq1 res)
        {
            string log = $"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} readSeq {msg.readSeq}";
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

            if (!state.roomDict.TryGetValue(msg.roomId, out UserFriendChatStateRoom? stateRoom))
            {
                this.service.logger.Error($"{log} stateRoom == null");
                return ECode.Success;
            }

            if (msg.readSeq > stateRoom.maxSeq)
            {
                this.service.logger.Error($"{log} msg.readSeq > stateRoom.maxSeq {stateRoom.maxSeq}");
                return ECode.Success;
            }

            if (msg.readSeq == stateRoom.maxSeq && stateRoom.unreadCount == 0)
            {
                return ECode.Success;
            }

            await this.server.userFriendChatStateProxy.SetReadSeq_DecreaseUnreadCount_1(user.userId, msg.roomId, msg.readSeq,
                msg.readSeq == stateRoom.maxSeq && stateRoom.unreadCount > 0
                    ? stateRoom.unreadCount
                    : null);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgAckFriendChatReadSeq1 msg, ECode e, ResAckFriendChatReadSeq1 res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}