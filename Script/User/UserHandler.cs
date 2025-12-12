using Data;

namespace Script
{
    public abstract class UserHandler<Msg, Res> : Handler<UserService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected UserHandler(Server server, UserService service) : base(server, service)
        {
        }


        public UserServiceData usData { get { return this.service.sd; } }

        public UserServiceScript usScript { get { return this.service.ss; } }

        public User? GetUser(IConnection connection)
        {
            if (connection is UserConnection userConnection)
            {
                return userConnection.GetUser();
            }

            return null;
        }
    }
}