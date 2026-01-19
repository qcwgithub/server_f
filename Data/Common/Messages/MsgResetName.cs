using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgResetName
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResResetName
    {
    }
}