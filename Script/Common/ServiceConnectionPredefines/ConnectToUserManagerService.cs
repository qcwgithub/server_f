using Data;

namespace Script
{
    public class ConnectToUserManagerService : ConnectToStatelessService
    {
        public ConnectToUserManagerService(Service self) : base(self, ServiceType.UserManager)
        {

        }
    }
}