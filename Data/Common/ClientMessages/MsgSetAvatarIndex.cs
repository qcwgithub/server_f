using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSetAvatarIndex
    {
        [Key(0)]
        public int avatarIndex;
    }

    [MessagePackObject]
    public class ResSetAvatarIndex
    {

    }
}