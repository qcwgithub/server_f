using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgLoadRoom
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public string? lockValue;
    }

    [MessagePackObject]
    public class ResLoadRoom
    {
        [Key(0)]
        public int serviceId;
        [Key(1)]
        public long expiry;
    }
}