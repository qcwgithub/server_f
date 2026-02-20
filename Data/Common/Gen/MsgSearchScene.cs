using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSearchScene
    {
        [Key(0)]
        public string keyword;
    }
}
