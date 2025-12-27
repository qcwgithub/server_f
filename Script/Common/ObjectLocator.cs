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
            return new ObjectLocator(server, service, locatorData, UserKey.Location);
        }

        public static ObjectLocator CreateRoomLocator(Server server, Service service, ObjectLocatorData locatorData)
        {
            return new ObjectLocator(server, service, locatorData, RoomKey.Location);
        }

        public async Task<stObjectLocation> GetLocation(long objectId)
        {
            long nowS = TimeUtils.GetTimeS();
            stObjectLocation location = this.locatorData.GetLocation(objectId, nowS);
            if (location.IsValid())
            {
                return location;
            }

            location = await this.locationRedis.GetLocation(objectId);
            if (!location.IsValid())
            {
                return location;
            }

            this.locatorData.SaveLocation(objectId, location);
            return location;
        }

        public void CacheLocation(long objectId, stObjectLocation location)
        {
            this.locatorData.SaveLocation(objectId, location);
        }
    }
}