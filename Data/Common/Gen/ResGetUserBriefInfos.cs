using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ResGetUserBriefInfos
    {
        [Key(0)]
        public List<UserBriefInfo> userBriefInfos;
    }
}
