using Data;

namespace Script
{
    public class User_GetRoomChatHistory : Handler<UserService, MsgGetRoomChatHistory, ResGetRoomChatHistory>
    {
        public override MsgType msgType => MsgType.GetRoomChatHistory;
        public User_GetRoomChatHistory(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgGetRoomChatHistory msg, ResGetRoomChatHistory res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} lastMessageId {msg.lastMessageId}");

            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            if (msg.lastMessageId <= 0)
            {
                return ECode.InvalidParam;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.roomId != user.roomId)
            {
                return ECode.WrongRoomId;
            }

            var roomMessageConfig = this.server.data.serverConfig.roomMessageConfig;
            res.history = await this.server.roomMessagesRedis.GetHistory(msg.roomId, msg.lastMessageId, roomMessageConfig.getHistoryMessageCount);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetRoomChatHistory msg, ECode e, ResGetRoomChatHistory res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}