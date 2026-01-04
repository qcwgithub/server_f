using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomUserEnter
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public long roomId;
        [Key(2)]
        public int gatewayServiceId;
    }

    [MessagePackObject]
    public class ResRoomUserEnter
    {
        
    }
}