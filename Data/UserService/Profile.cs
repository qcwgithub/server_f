using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class Profile
    {
        [Key(0)]
        public long createTimestamp;
        [Key(1)]
        public string name;
    }
}