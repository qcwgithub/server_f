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
    }
}