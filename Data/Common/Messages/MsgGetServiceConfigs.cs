using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetServiceConfigs
    {
        [Key(0)]
        public ServiceType fromServiceType;
        [Key(1)]
        public int fromServiceId;
        [Key(2)]
        public string why;
    }

    [MessagePackObject]
    public class ResGetServiceConfigs
    {
        [Key(0)]
        public int majorVersion;
        [Key(1)]
        public int minorVerson;
        [Key(2)]
        public bool open;
        [Key(4)]
        public List<ServiceConfig> serviceConfigs;

        public void VisitServiceConfig(ServiceType serviceType, Action<ServiceConfig> action)
        {
            foreach (ServiceConfig sc in this.serviceConfigs)
            {
                if (sc.serviceType == serviceType)
                {
                    action(sc);
                }
            }
        }

        public ServiceConfig FindServiceConfig(ServiceType serviceType, int serviceId)
        {
            return this.serviceConfigs.Find(x => x.serviceType == serviceType && x.serviceId == serviceId);
        }

        public bool ExistServiceType(ServiceType serviceType)
        {
            return this.serviceConfigs.Exists(x => x.serviceType == serviceType);
        }
    }

    [MessagePackObject]
    public class A_ResGetServiceConfigs
    {
        [Key(0)]
        public ResGetServiceConfigs res;
    }
}