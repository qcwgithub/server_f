namespace Script
{
    public class Auth_Shutdown : OnShutdown<AuthService>
    {
        public Auth_Shutdown(Server server, AuthService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {

        }
    }
}