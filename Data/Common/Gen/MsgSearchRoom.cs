using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSearchRoom
    {
        [Key(0)]
        public string keyword;
    }
}
