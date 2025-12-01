using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetReloadConfigOptions
    {

    }

    [MessagePackObject]
    public class ResGetReloadConfigOptions
    {
        [Key(0)]
        public List<string> files;
    }
}