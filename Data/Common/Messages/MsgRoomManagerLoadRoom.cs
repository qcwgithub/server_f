using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerLoadRoom
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResRoomManagerLoadRoom
    {
        [Key(0)]
        public stObjectLocation location;
    }
}