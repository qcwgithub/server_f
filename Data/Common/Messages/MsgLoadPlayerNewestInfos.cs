using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgLoadPlayerNewestInfos
    {
        [Key(0)]
        public LoadPlayerNewestWhat what;
        [Key(1)]
        public List<long> playerIds;
        [Key(2)]
        public bool fillDefaultSide;
    }

    [MessagePackObject]
    public class ResLoadPlayerNewestInfos
    {
        [Key(0)]
        public List<PlayerNewestInfo> newestInfos;
    }
}