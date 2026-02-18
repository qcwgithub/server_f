using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetUserBriefInfos
    {
        [Key(0)]
        public HashSet<long> userIds;
    }
}
