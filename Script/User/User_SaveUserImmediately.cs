using Data;
namespace Script
{
    public class User_SaveUserImmediately : UserHandler<MsgSaveUserImmediately, ResSaveUserImmediately>
    {
        public User_SaveUserImmediately(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_SaveUserImmediately;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveUserImmediately msg, ResSaveUserImmediately res)
        {
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                this.service.logger.Error($"{this.msgType} userId {msg.userId} user == null");
                return ECode.UserNotExist;
            }

            ECode e = await this.service.SaveUser(user, msg.reason);
            return e;
        }

        public override void PostHandle(MessageContext context, MsgSaveUserImmediately msg, ECode e, ResSaveUserImmediately res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}