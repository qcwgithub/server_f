using Data;

namespace Script
{
    public abstract class AuthHandler<Msg, Res> : Handler<AuthService, Msg, Res>
        where Msg : class
        where Res : class, new()
    {
        protected AuthHandler(Server server, AuthService service) : base(server, service)
        {
        }

        public AuthServiceData sd { get { return this.service.sd; } }
    }
}