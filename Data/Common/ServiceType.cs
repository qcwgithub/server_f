namespace Data
{
    public enum ServiceType
    {
        Gateway,
        Db,
        User,
        Global,
        Command,
        Auth,
        Count,
    }

    public static class ServiceTypeExt
    {
        public static bool ShouldLogErrorWhenDisconnectFrom(this ServiceType self, ServiceType remote)
        {
            if (self.IsCommand())
            {
                return false;
            }

            if (remote.IsCommand())
            {
                return false;
            }

            return true;
        }

        public static bool ShouldSendFeiShuWhenLogError(this ServiceType self)
        {
            if (self.IsCommand())
            {
                return false;
            }

            return true;
        }

        public static bool IsCommand(this ServiceType self)
        {
            return self == ServiceType.Command;
        }

        public static bool ReleaseModeLogToConsole(this ServiceType self)
        {
            if (self.IsCommand())
            {
                return true;
            }
            return false;
        }

        public static bool IsDBService(this ServiceType self)
        {
            return self == ServiceType.Db;
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