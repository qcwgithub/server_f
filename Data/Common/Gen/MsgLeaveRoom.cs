using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgLeaveRoom
    {
        [Key(0)]
        public long roomId;
    }
}
