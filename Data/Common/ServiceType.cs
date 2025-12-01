using System;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Data
{
    public enum ServiceType
    {
        DBPlayer,   // 200
        Player,     // 400
        Monitor,    // 4
        GAAA,       // 900
        GStateless, // 1000
        Stateless,  // 1100
        ConfigManager,  // 1200
        Count,
    }

    public static class ServiceTypeExt
    {
        public static bool ShouldLogErrorWhenDisconnectFrom(this ServiceType self, ServiceType remote)
        {
            if (self.IsMonitor())
            {
                return false;
            }

            if (remote.IsMonitor())
            {
                return false;
            }

            return true;
        }

        public static bool ShouldSendFeiShuWhenLogError(this ServiceType self)
        {
            if (self.IsMonitor())
            {
                return false;
            }

            return true;
        }

        public static bool IsMonitor(this ServiceType self)
        {
            return self == ServiceType.Monitor;
        }

        public static bool ReleaseModeLogToConsole(this ServiceType self)
        {
            if (self.IsMonitor())
            {
                return true;
            }
            return false;
        }

        public static bool IsDBService(this ServiceType self)
        {
            return self == ServiceType.DBPlayer;
        }
    }

    public struct ServiceTypeAndId
    {
        public ServiceType serviceType;
        public int serviceId;

        public override string ToString()
        {
            return this.serviceType.ToString() + this.serviceId;
        }

        public static ServiceTypeAndId FromString(string raw)
        {
            var typeAndId = new ServiceTypeAndId();
            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c >= '0' && c <= '9')
                {
                    typeAndId.serviceId = int.Parse(raw.Substring(i));
                    typeAndId.serviceType = Enum.Parse<ServiceType>(raw.Substring(0, i));
                    break;
                }
            }
            return typeAndId;
        }

        public static ServiceTypeAndId Create(ServiceType serviceType, int serviceId)
        {
            return new ServiceTypeAndId { serviceType = serviceType, serviceId = serviceId };
        }
    }
}