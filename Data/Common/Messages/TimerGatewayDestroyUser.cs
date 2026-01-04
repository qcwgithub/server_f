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

    public class TimerGatewayDestroyUser
    {
        public long userId;
        public GatewayDestroyUserReason reason;
        public MsgKick? msgKick;
    }
}