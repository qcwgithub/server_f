using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSaveRoomImmediately
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public string reason;
    }

    public class ResSaveRoomImmediately
    {

    }
}