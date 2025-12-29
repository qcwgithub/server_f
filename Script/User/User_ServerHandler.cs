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
    }
}