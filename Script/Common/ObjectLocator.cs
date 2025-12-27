using Data;

namespace Script
{
    public class ObjectLocator : ServiceScript<Service>
    {
        public readonly ObjectLocatorData locatorData;
        public readonly ObjectLocationRedisRW locationRedis;
        public ObjectLocator(Server server, Service service, ObjectLocatorData locatorData, ObjectLocationRedis.GetKeyFunc getKeyFunc) : base(server, service)
        {
            this.locatorData = locatorData;
            this.locationRedis = new ObjectLocationRedisRW(server, getKeyFunc);
        }

        public async Task<int> GetOwningServiceId(long objectId)
        {
            long nowS = TimeUtils.GetTimeS();
            int serviceId = this.locatorData.GetOwningServiceId(objectId, nowS);
            if (serviceId != 0)
            {
                return serviceId;
            }

            long expiry;
            (serviceId, expiry) = await this.locationRedis.GetOwningServiceId(objectId);
            if (serviceId == 0)
            {
                return 0;
            }

            this.locatorData.Update(objectId, serviceId, expiry);
            return serviceId;
        }

        public void CacheOwningServiceId(long objectId, int serviceId, int ttl)
        {
            long nowS = TimeUtils.GetTimeS();
            this.locatorData.Update(objectId, serviceId, nowS + ttl);
        }
    }
}