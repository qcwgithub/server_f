using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetDefaultAvatar
    {
        [Key(0)]
        public int defaultAvatarIndex;
    }

    [MessagePackObject]
    public class ResSetDefaultAvatar
    {

    }
}