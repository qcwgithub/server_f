using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetScriptVersion
    {
        
    }

    [MessagePackObject]
    public class ResGetScriptVersion
    {
        [Key(0)]
        public string version;
    }
}