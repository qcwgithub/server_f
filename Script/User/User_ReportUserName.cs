using Data;

namespace Script
{
    public class User_ReportUserName : Handler<UserService, MsgReportUserName, ResReportUserName>
    {
        public override MsgType msgType => MsgType.ReportUserName;
        public User_ReportUserName(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReportUserName msg, ResReportUserName res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} targetUserId {msg.targetUserId} reason {msg.reason}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.reason < 0 || msg.reason >= UserNameReportReason.Count)
            {
                return ECode.InvalidParam;
            }

            var info = new UserNameReportInfo();
            info.reportUserId = user.userId;
            info.reason = msg.reason;
            info.timeS = TimeUtils.GetTimeS();
            info.targetUserName = string.Empty; // todo

            var msgDb = new MsgSave_UserNameReportInfo();
            msgDb.info = info;

            await this.service.dbServiceProxy.Save_UserNameReportInfo(msgDb);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReportUserName msg, ECode e, ResReportUserName res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}