using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResSetName
    {
        [Key(0)]
        public string userName;
    }
}
