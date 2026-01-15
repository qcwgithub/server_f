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
        // Gatewaay
        [Key(4)]
        public string outIp;
        // Gateway
        [Key(5)]
        public int outPort;
        // UserManager
        [Key(6)]
        public long userIdSnowflakeWorkerId;
        // RoomManager
        [Key(7)]
        public long roomIdSnowflakeWorkerId;
        // Room
        [Key(8)]
        public long roomMessageIdSnowflakeWorkerId;

        [IgnoreMember]
        public ServiceTypeAndId tai
        {
            get
            {
                return ServiceTypeAndId.Create(this.serviceType, this.serviceId);
            }
        }
    }
}