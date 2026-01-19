using Data;

namespace Script
{
    public class User_ReportUser : Handler<UserService, MsgReportUser, ResReportUser>
    {
        public override MsgType msgType => MsgType.ReportUser;
        public User_ReportUser(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgReportUser msg, ResReportUser res)
        {
            this.service.logger.Info($"{this.msgType} userId {context.msg_userId} targetUserId {msg.targetUserId} reason {msg.reason}");

            User? user = await this.service.LockUser(context.msg_userId, context);
            if (user == null)
            {
                return ECode.UserNotExist;
            }

            if (msg.reason < 0 || msg.reason >= UserReportReason.Count)
            {
                return ECode.InvalidParam;
            }

            var info = new UserReportInfo();
            info.reportUserId = user.userId;
            info.reason = msg.reason;
            info.timeS = TimeUtils.GetTimeS();

            var msgDb = new MsgSave_UserReportInfo();
            msgDb.info = info;

            await this.service.dbServiceProxy.Save_UserReportInfo(msgDb);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgReportUser msg, ECode e, ResReportUser res)
        {
            this.service.TryUnlockUser(context.msg_userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}