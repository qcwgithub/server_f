using Data;

namespace Script
{
    public class UserMessageDispatcher : MessageDispatcher
    {
        public UserMessageDispatcher(Server server, Service service) : base(server, service)
        {
        }

        protected override ECode BeforeHandle(MessageContext context, MsgType type, object msg)
        {
            ECode e = base.BeforeHandle(context, type, msg);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }

        protected override void AfterHandle(MessageContext context, MsgType type, object msg, MyResponse r)
        {
            if (context.connection is UserConnection userConnection &&
                r.e != ECode.Success && type.LogErrorIfNotSuccess())
            {
                this.service.logger.ErrorFormat("{0} userIdId {1} ECode.{2}", type, userConnection.user.userId, r.e);
            }
            else
            {
                base.AfterHandle(context, type, msg, r);
            }
        }

        protected override void BeforePostHandle(MessageContext context, MsgType type, object msg, MyResponse r)
        {
            base.BeforePostHandle(context, type, msg, r);
        }
    }
}