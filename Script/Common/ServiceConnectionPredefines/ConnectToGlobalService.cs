using Data;

namespace Script
{
    public class ConnectToGlobalService : ConnectToStatefulService
    {
        public ConnectToGlobalService(Service self) : base(self, ServiceType.Global)
        {

        }
    }
}