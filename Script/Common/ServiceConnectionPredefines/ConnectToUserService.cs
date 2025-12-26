using Data;

namespace Script
{
    public class ConnectToUserService : ConnectWithUserService
    {
        public ConnectToUserService(Service self) : base(self)
        {
            MyDebug.Assert(self.data.connectToServiceTypes.Contains(ServiceType.User));
        }
    }
}