using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerPrepareCreateRoom
    {
        [Key(0)]
        public RoomType roomType;
    }

    [MessagePackObject]
    public class ResRoomManagerPrepareCreateRoom
    {
        [Key(0)]
        public long roomId;
    }
}