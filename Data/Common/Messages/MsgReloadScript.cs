using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgReloadScript
    {
        [Key(0)]
        public bool local;
        [Key(1)]
        public byte[]? dllBytes;
        [Key(2)]
        public byte[]? pdbBytes;
    }

    [MessagePackObject]
    public class ResReloadScript
    {
        [Key(0)]
        public string? message;
    }
}