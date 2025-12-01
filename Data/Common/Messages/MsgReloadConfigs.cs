using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgReloadConfigs
    {
        [Key(0)]
        public bool all;
        [Key(1)]
        public List<string> files; // when all = false
    }

    [MessagePackObject]
    public class ResReloadConfigs
    {

    }
}