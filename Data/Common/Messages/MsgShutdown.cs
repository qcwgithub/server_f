using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgShutdown
    {
        [Key(0)]
        public bool force;
    }

    [MessagePackObject]
    public class ResShutdown
    {

    }
}