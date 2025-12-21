using Data;

namespace Script
{
    public class UserMessageDispatcher : MessageDispatcher
    {
        public UserMessageDispatcher(Server server, Service service) : base(server, service)
        {
        }

        protected override ECode BeforeHandle(IConnection connection, MsgType type, object msg)
        {
            ECode e = base.BeforeHandle(connection, type, msg);
            if (e != ECode.Success)
            {
                return e;
            }

            if (connection is UserConnection userConnection)
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

        protected override void AfterHandle(IConnection connection, MsgType type, object msg, ECode e, object res)
        {
            if (connection is UserConnection userConnection &&
                e != ECode.Success && type.LogErrorIfNotSuccess())
            {
                this.service.logger.ErrorFormat("{0} userIdId {1} ECode.{2}", type, userConnection.user.userId, e);
            }
            else
            {
                base.AfterHandle(connection, type, msg, e, res);
            }
        }

        protected override void BeforePostHandle(IConnection connection, MsgType type, object msg, ECode e, object res)
        {
            base.BeforePostHandle(connection, type, msg, e, res);

            if (connection is UserConnection userConnection)
            {
                userConnection.msgProcessing = 0;
            }
        }
    }
}