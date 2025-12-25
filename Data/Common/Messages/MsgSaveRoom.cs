using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSaveRoom
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        public string reason;
    }

    [MessagePackObject]
    public class ResSaveRoom
    {
        
    }
}