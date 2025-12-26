using Data;

namespace Script
{
    public class ConnectToGatewayService : ConnectToStatefulService
    {
        public ConnectToGatewayService(Service self) : base(self, ServiceType.Gateway)
        {

        }
    }
}