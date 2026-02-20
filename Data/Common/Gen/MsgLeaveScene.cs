using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgLeaveScene
    {
        [Key(0)]
        public long roomId;
    }
}
