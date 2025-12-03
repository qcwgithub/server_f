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
        public string purpose;
        [Key(1)]
        public int majorVersion;
        [Key(2)]
        public int minorVerson;
        [Key(3)]
        public List<ServiceConfig> allServiceConfigs;

        public void VisitServiceConfig(ServiceType serviceType, Action<ServiceConfig> action)
        {
            foreach (ServiceConfig sc in this.allServiceConfigs)
            {
                if (sc.serviceType == serviceType)
                {
                    action(sc);
                }
            }
        }

        public ServiceConfig FindServiceConfig(ServiceType serviceType, int serviceId)
        {
            return this.allServiceConfigs.Find(x => x.serviceType == serviceType && x.serviceId == serviceId);
        }

        public bool ExistServiceType(ServiceType serviceType)
        {
            return this.allServiceConfigs.Exists(x => x.serviceType == serviceType);
        }
    }

    [MessagePackObject]
    public class A_ResGetServiceConfigs
    {
        [Key(0)]
        public ResGetServiceConfigs res;
    }
}