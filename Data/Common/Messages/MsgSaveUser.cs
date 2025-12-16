using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSaveUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string reason;
    }

    [MessagePackObject]
    public class ResSaveUser
    {
        
    }
}