using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ServiceConfig
    {
        [Key(0)]
        public ServiceType serviceType;
        [Key(1)]
        public int serviceId;
        [Key(2)]
        public string inIp;
        [Key(3)]
        public int inPort;
        // GAAA | Player
        [Key(4)]
        public string outIp;
        // GAAA | Player
        [Key(5)]
        public int outPort;

        public ServiceTypeAndId Tai()
        {
            return ServiceTypeAndId.Create(this.serviceType, this.serviceId);
        }
    }
}