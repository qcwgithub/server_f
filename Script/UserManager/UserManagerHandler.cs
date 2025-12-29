using Data;

namespace Script
{
    public abstract class UserManagerHandler<Msg, Res> : Handler<UserManagerService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected UserManagerHandler(Server server, UserManagerService service) : base(server, service)
        {
        }
    }
}