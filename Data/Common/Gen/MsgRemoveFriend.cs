using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRemoveFriend
    {
        [Key(0)]
        public long friendUserId;
    }
}
