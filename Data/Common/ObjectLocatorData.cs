namespace Data
{
    public struct ServiceIdAndExpiry
    {
        public int serviceId;
        public long expiry;
    }

    public class ObjectLocatorData
    {
        public long lastUpdateS;
        public readonly Dictionary<long, ServiceIdAndExpiry> cache;
        public ObjectLocatorData()
        {
            this.cache = new Dictionary<long, ServiceIdAndExpiry>();
        }

        public void Update(long objectId, int serviceId, long expiry)
        {
            this.cache[objectId] = new ServiceIdAndExpiry { serviceId = serviceId, expiry = expiry };
        }

        public (int, long) GetOwningServiceIdWithExpiry(long objectId, long nowS)
        {
            if (!this.cache.TryGetValue(objectId, out ServiceIdAndExpiry st) || st.expiry < nowS)
            {
                return (0, 0);
            }
            return (st.serviceId, st.expiry);
        }
    }
}