namespace Data
{
    public enum GatewayDestroyUserReason
    {
        DestroyTimer_Disconnect,
        ServerKick,
        Shutdown,
    }

    public enum GatewayClearDestroyTimerReason
    {
        UserLogin,
        Destroy,
    }

    public class TimerGatewayDestroyUser
    {
        public long userId;
        public GatewayDestroyUserReason reason;
        public MsgKick? msgKick;
    }
}