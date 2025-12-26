using Data;

namespace Script
{
    public abstract class User_ServerHandler<Msg, Res> : Handler<UserService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected User_ServerHandler(Server server, UserService service) : base(server, service)
        {
        }

        public override sealed async Task<ECode> Handle(IConnection connection, Msg msg, Res res)
        {
            ServiceConnection? serviceConnection = connection as ServiceConnection;
            if (serviceConnection == null)
            {
                this.service.logger.Error($"{this.msgType} serviceConnection == null");
                return ECode.Error;
            }

            return await this.Handle(serviceConnection, msg, res);
        }

        protected abstract Task<ECode> Handle(ServiceConnection connection, Msg msg, Res res);
    }
}