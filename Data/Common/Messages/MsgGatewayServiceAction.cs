using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGatewayServiceAction
    {
        [Key(0)]
        public int? destroyTimeoutS;
    }

    [MessagePackObject]
    public class ResGatewayServiceAction
    {

    }
}