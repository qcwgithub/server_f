using System.Collections.Generic;
using MessagePack;
namespace Data
{
    public enum UserDestroyUserReason
    {
        DestroyTimer_GatewayDisconnect,
        DestroyTimer_DisconnectFromGateway,
        Shutdown,
        ServerKick,
        SimulateLogin,
    }

    public enum UserClearDestroyTimerReason
    {
        UserLoginSuccess,
        Destroy,
    }

    [MessagePackObject]
    public class TimerDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public UserDestroyUserReason reason;
    }
}