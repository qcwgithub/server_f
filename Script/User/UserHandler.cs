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

        
    }
}