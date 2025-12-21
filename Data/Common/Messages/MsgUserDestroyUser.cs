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
    }

    public enum UserClearDestroyTimerReason
    {
        UserLoginSuccess,
    }

    [MessagePackObject]
    public class MsgUserDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public UserDestroyUserReason reason;
    }

    [MessagePackObject]
    public class ResUserDestroyUser
    {

    }
}