using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ConnectorInfo
    {
        [Key(0)]
        public ServiceType serviceType;
        [Key(1)]
        public int serviceId;
        [Key(2)]
        public ServiceState serviceState;
    }

    [MessagePackObject]
    public class MsgConnectorInfo
    {
        [Key(0)]
        public ConnectorInfo connectorInfo;
        [Key(1)]
        public bool isCommand;
    }

    [MessagePackObject]
    public class ResConnectorInfo
    {

    }
}