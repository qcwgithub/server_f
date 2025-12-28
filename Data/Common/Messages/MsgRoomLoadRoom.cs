using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomLoadRoom
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResRoomLoadRoom
    {
        [Key(0)]
        public RoomInfo roomInfo;
    }
}