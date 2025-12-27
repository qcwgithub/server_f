using Data;

namespace Script
{
    public class ObjectLocator : ServiceScript<Service>
    {
        public readonly ObjectLocatorData locatorData;
        public readonly ObjectLocationRedisRW locationRedis;
        private ObjectLocator(Server server, Service service, ObjectLocatorData locatorData, ObjectLocationRedis.GetKeyFunc getKeyFunc) : base(server, service)
        {
            this.locatorData = locatorData;
            this.locationRedis = new ObjectLocationRedisRW(server, getKeyFunc);
        }

        public static ObjectLocator CreateUserLocator(Server server, Service service, ObjectLocatorData locatorData)
        {
            return new ObjectLocator(server, service, locatorData, UserKey.OwningServiceId);
        }

        public static ObjectLocator CreateRoomLocator(Server server, Service service, ObjectLocatorData locatorData)
        {
            return new ObjectLocator(server, service, locatorData, RoomKey.OwningServiceId);
        }

        public async Task<int> GetOwningServiceId(long objectId)
        {
            (int serviceId, long expiry) = await this.GetOwningServiceIdWithExpiry(objectId);
            return serviceId;
        }

        public async Task<(int, long)> GetOwningServiceIdWithExpiry(long objectId)
        {
            long nowS = TimeUtils.GetTimeS();
            (int serviceId, long expiry) = this.locatorData.GetOwningServiceIdWithExpiry(objectId, nowS);
            if (serviceId != 0)
            {
                return (serviceId, expiry);
            }

            (serviceId, expiry) = await this.locationRedis.GetOwningServiceIdWithExpiry(objectId);
            if (serviceId == 0)
            {
                return (0, 0);
            }

            this.locatorData.Update(objectId, serviceId, expiry);
            return (serviceId, expiry);
        }

        public void SaveOwningServiceId(long objectId, int serviceId, long expiry)
        {
            this.locatorData.Update(objectId, serviceId, expiry);
        }
    }
}