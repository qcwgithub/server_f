using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgViewMongoDumpList
    {
        [Key(0)]
        public string dir;
        [Key(1)]
        public int count;
    }

    [MessagePackObject]
    public class ResViewMongoDumpList
    {
        [Key(0)]
        public List<string> directories;
    }
}