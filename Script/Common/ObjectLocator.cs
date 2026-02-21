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

        public async Task<stObjectLocation[]> GetLocations(List<long> objectIds)
        {
            long nowS = TimeUtils.GetTimeS();
            var locations = new stObjectLocation[objectIds.Count];
            List<(int, long)>? missing = null;

            for (int i = 0; i < objectIds.Count; i++)
            {
                long objectId = objectIds[i];
                stObjectLocation location = this.locatorData.GetLocation(objectId, nowS);
                if (location.IsValid())
                {
                    locations[i] = location;
                }
                else
                {
                    if (missing == null)
                    {
                        missing = new List<(int, long)>();
                    }
                    missing.Add((i, objectId));
                }
            }

            if (missing != null)
            {
                var locs = await this.locationRedis.GetLocations(missing.Select(x => x.Item2));
                for (int i = 0; i < missing.Count; i++)
                {
                    locations[missing[i].Item1] = locs[i];
                    if (locs[i].IsValid())
                    {
                        this.locatorData.SaveLocation(missing[i].Item2, locs[i]);
                    }
                }
            }

            return locations;
        }

        public void CacheLocation(long objectId, stObjectLocation location)
        {
            this.locatorData.SaveLocation(objectId, location);
        }
    }
}