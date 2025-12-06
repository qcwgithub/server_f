using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetServiceState
    {
        
    }

    [MessagePackObject]
    public class ResGetServiceState
    {
        [Key(0)]
        public ServiceType serviceType;

        [Key(1)]
        public int serviceId;

        [Key(2)]
        public ServiceState serviceState;
    }
}