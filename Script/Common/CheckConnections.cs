using Data;

namespace Script
{
    public partial class Service
    {
        public virtual async Task<ECode> CheckConnections()
        {
            if (this.data.connectToServiceTypes.Count == 0)
            {
                return ECode.Success;
            }

            foreach (ServiceType to_serviceType in this.data.connectToServiceTypes)
            {
                this.data.current_resGetServiceConfigs.VisitServiceConfig(to_serviceType, sc =>
                {
                    if (sc.serviceId == this.data.serviceId)
                    {
                        return;
                    }

                    this.GetServiceConnectionOrConnect(sc.serviceType, sc.serviceId, sc.inIp, sc.inPort);
                });
            }

            return ECode.Success;
        }
    }
}