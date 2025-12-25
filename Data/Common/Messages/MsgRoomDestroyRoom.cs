using System.Collections.Generic;
using MessagePack;
namespace Data
{
    public enum RoomDestroyRoomReason
    {
        DestroyTimer_GatewayDisconnect,
        DestroyTimer_DisconnectFromGateway,
        Shutdown,
        ServerKick,
    }

    public enum RoomClearDestroyTimerReason
    {
        RoomLoginSuccess,
    }

    [MessagePackObject]
    public class MsgRoomDestroyRoom
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public RoomDestroyRoomReason reason;
    }

    [MessagePackObject]
    public class ResRoomDestroyRoom
    {

    }
}