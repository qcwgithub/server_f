using Data;

namespace Script
{
    public class Auth_Start : OnStart<AuthService>
    {
        public Auth_Start(Server server, AuthService service) : base(server, service)
        {
        }

        protected override async Task<ECode> Handle2()
        {
            return ECode.Success;
        }
    }
}