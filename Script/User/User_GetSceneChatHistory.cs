using Data;

namespace Script
{
    [AutoRegister]
    public class User_GetSceneChatHistory : Handler<UserService, MsgGetSceneChatHistory, ResGetSceneChatHistory>
    {
        public override MsgType msgType => MsgType.GetSceneChatHistory;
        public User_GetSceneChatHistory(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgGetSceneChatHistory msg, ResGetSceneChatHistory res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} lastSeq {msg.lastSeq}");

            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            if (msg.lastSeq <= 0)
            {
                return ECode.InvalidParam;
            }

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.roomId != user.sceneId)
            {
                return ECode.WrongRoomId;
            }

            var roomMessageConfig = this.server.data.serverConfig.sceneMessageConfig;
            res.history = await this.server.sceneMessagesRedis.GetHistory(msg.roomId, msg.lastSeq, roomMessageConfig.getHistoryMessageCount);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgGetSceneChatHistory msg, ECode e, ResGetSceneChatHistory res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}