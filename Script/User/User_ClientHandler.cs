using Data;

namespace Script
{
    public abstract class User_ClientHandler<Msg, Res> : Handler<UserService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected User_ClientHandler(Server server, UserService service) : base(server, service)
        {
        }

        public override sealed async Task<ECode> Handle(IConnection connection, Msg msg, Res res)
        {
            UserConnection? userConnection = connection as UserConnection;
            if (userConnection == null)
            {
                this.service.logger.Error($"{this.msgType} serviceConnection == null");
                return ECode.Error;
            }

            return await this.Handle(userConnection, msg, res);
        }

        protected abstract Task<ECode> Handle(UserConnection connection, Msg msg, Res res);
    }
}