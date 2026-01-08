using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        ServiceConfig? FindServiceConfig(string taiStr)
        {
            ServiceTypeAndId tai = ServiceTypeAndId.FromString(taiStr);
            ServiceConfig? serviceConfig = this.allServiceConfigs.Find(x => x.serviceType == tai.serviceType && x.serviceId == tai.serviceId);
            if (serviceConfig == null)
            {
                ConsoleEx.WriteLine(ConsoleColor.Red, $"serviceConfig == null, tai {tai}");
                return null;
            }

            return serviceConfig;
        }

        List<ServiceConfig> FindServiceConfigs(List<string> taiStrList)
        {
            List<ServiceConfig> serviceConfigs = new();
            foreach (string taiStr in taiStrList)
            {
                ServiceConfig? serviceConfig = this.FindServiceConfig(taiStr);
                if (serviceConfig != null)
                {
                    serviceConfigs.Add(serviceConfig);
                }

            }
            return serviceConfigs;
        }

        List<ServiceConfig> FindServiceConfigs(List<string[]> taiStrArrayList)
        {
            List<ServiceConfig> serviceConfigs = new();
            foreach (string[] taiStrArray in taiStrArrayList)
            {
                foreach (string taiStr in taiStrArray)
                {
                    ServiceConfig? serviceConfig = this.FindServiceConfig(taiStr);
                    if (serviceConfig != null)
                    {
                        serviceConfigs.Add(serviceConfig);
                    }
                }
            }
            return serviceConfigs;
        }
    }
}