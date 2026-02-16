using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgBlockUser
    {
        [Key(0)]
        public long targetUserId;
    }
}
