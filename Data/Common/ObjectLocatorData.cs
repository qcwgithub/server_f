namespace Data
{
    public class ObjectLocatorData
    {
        public readonly Dictionary<long, stObjectLocation> cache;
        public ObjectLocatorData()
        {
            this.cache = new Dictionary<long, stObjectLocation>();
        }

        public void SaveLocation(long objectId, stObjectLocation location)
        {
            this.cache[objectId] = location;
        }

        public stObjectLocation GetLocation(long objectId, long nowS)
        {
            if (!this.cache.TryGetValue(objectId, out stObjectLocation location))
            {
                return default;
            }

            if (location.expiry < nowS)
            {
                this.cache.Remove(objectId);
                return default;
            }

            return location;
        }
    }
}