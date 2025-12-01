using System.Collections.Generic;
using MessagePack;

namespace Data
{

    [MessagePackObject]
    public class MsgGetConnectedInfos
    {
    }

    [MessagePackObject]
    public class ResGetConnectedInfos
    {
        [Key(0)]
        public List<ServiceTypeAndId> connectedInfos;
    }
}