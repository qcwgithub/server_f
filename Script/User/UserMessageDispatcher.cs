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

            if (context.connection is UserConnection userConnection)
            {
                if (userConnection.msgProcessing != 0 && !type.CanParallel())
                {
                    this.service.logger.ErrorFormat("MsgType.{0} wait MsgType.{1}", type, userConnection.msgProcessing);
                    return ECode.MsgProcessing;
                }

                if (!type.CanParallel())
                {
                    userConnection.msgProcessing = type;
                }
            }

            return ECode.Success;
        }

        protected override void AfterHandle(MessageContext context, MsgType type, object msg, ECode e, object res)
        {
            if (context.connection is UserConnection userConnection &&
                e != ECode.Success && type.LogErrorIfNotSuccess())
            {
                this.service.logger.ErrorFormat("{0} userIdId {1} ECode.{2}", type, userConnection.user.userId, e);
            }
            else
            {
                base.AfterHandle(context, type, msg, e, res);
            }
        }

        protected override void BeforePostHandle(MessageContext context, MsgType type, object msg, ECode e, object res)
        {
            base.BeforePostHandle(context, type, msg, e, res);

            if (context.connection is UserConnection userConnection)
            {
                userConnection.msgProcessing = 0;
            }
        }
    }
}