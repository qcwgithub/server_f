using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgOnConnectComplete
    {
        [Key(0)]
        public ServiceType to_serviceType;
        [Key(1)]
        public int to_serviceId;
    }
}