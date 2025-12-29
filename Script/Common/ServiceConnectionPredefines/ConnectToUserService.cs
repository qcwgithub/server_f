using Data;

namespace Script
{
    public class ConnectToUserService : ConnectWithUserService
    {
        public ConnectToUserService(Service self) : base(self, ServiceType.User)
        {
            MyDebug.Assert(self.data.connectToServiceTypes.Contains(ServiceType.User));
        }
    }
}