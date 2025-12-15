namespace Script
{
    public class UserManager_Shutdown : OnShutdown<UserManagerService>
    {
        public UserManager_Shutdown(Server server, UserManagerService service) : base(server, service)
        {
        }

        protected override async Task StopBusinesses()
        {

        }
    }
}