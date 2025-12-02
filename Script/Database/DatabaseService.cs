using Data;

namespace Script
{
    public class DatabaseService : Service
    {
        public DatabaseServiceData databaseServiceData
        {
            get
            {
                return (DatabaseServiceData)this.data;
            }
        }

        public DatabaseService(Server server, int serviceId) : base(server, serviceId)
        {
        }
    }
}