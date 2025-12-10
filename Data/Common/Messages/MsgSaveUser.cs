using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSaveUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        required public string place;
    }

    [MessagePackObject]
    public class ResSaveUser
    {
        
    }
}