using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgUnblockUser
    {
        [Key(0)]
        public long targetUserId;
    }
}
