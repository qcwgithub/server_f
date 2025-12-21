using System.Collections.Generic;
using MessagePack;
namespace Data
{
    public enum GatewayDestroyUserReason
    {
        DestroyTimer_Disconnect,
        ServerKick,
    }

    public enum GatewayClearDestroyTimerReason
    {
        Destroy,
    }

    [MessagePackObject]
    public class MsgGatewayDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public GatewayDestroyUserReason reason;
        [Key(2)]
        public MsgKick msgKick;
    }

    [MessagePackObject]
    public class ResGatewayDestroyUser
    {

    }
}