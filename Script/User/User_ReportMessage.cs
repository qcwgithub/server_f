using Data;

namespace Script
{
    [AutoRegister]
    public class User_ReportMessage : Handler<UserService, MsgReportMessage, ResReportMessage>
    {
        public override MsgType msgType => MsgType.ReportMessage;
        public User_ReportMessage(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReportMessage msg, ResReportMessage res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} roomId {msg.roomId} seq {msg.seq} reason {msg.reason}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }
            if (msg.roomId <= 0)
            {
                return ECode.InvalidRoomId;
            }

            if (msg.roomId != user.sceneId)
            {
                return ECode.WrongRoomId;
            }

            if (msg.reason < 0 || msg.reason >= MessageReportReason.Count)
            {
                return ECode.InvalidParam;
            }

            if (msg.seq <= 0)
            {
                return ECode.InvalidParam;
            }

            ChatMessage? message = await this.server.sceneMessagesRedis.QueryOne(msg.roomId, msg.seq);

            var info = new MessageReportInfo();
            info.reportUserId = user.userId;
            info.targetUserId = message == null ? 0 : message.senderId;
            info.roomId = user.sceneId;
            info.seq = msg.seq;
            info.reason = msg.reason;
            info.timeS = TimeUtils.GetTimeS();

            var msgDb = new MsgSave_MessageReportInfo();
            msgDb.info = info;

            await this.service.dbServiceProxy.Save_MessageReportInfo(msgDb);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReportMessage msg, ECode e, ResReportMessage res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}