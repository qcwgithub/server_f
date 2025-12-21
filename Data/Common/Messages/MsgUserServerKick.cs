using MessagePack;


namespace Data
{
    [MessagePackObject]
    public class MsgUserServerKick
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResUserServerKick
    {
        [Key(0)]
        public bool kicked;
    }
}