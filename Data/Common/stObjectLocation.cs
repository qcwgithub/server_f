using MessagePack;

namespace Data
{
    [MessagePackObject]
    public struct stObjectLocation
    {
        [Key(0)]
        public int serviceId;
        [Key(1)]
        public long expiry;

        public bool IsValid()
        {
            return this.serviceId > 0;
        }
    }
}