using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerLoadRoom
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public string? lockValue;
    }

    [MessagePackObject]
    public class ResRoomManagerLoadRoom
    {
        [Key(0)]
        public stObjectLocation location;
    }
}