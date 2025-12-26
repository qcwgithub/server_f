using Data;

namespace Script
{
    public class ConnectToDbService : ConnectToStatelessService
    {
        public ConnectToDbService(Service self) : base(self, ServiceType.Db)
        {

        }
    }
}