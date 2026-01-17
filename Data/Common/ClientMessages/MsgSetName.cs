using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetName
    {
        [Key(0)]
        public string userName;
    }

    [MessagePackObject]
    public class ResSetName
    {
        [Key(0)]
        public string userName;
    }
}