using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgConnectionClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }

    [MessagePackObject]
    public class ResConnectionClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }
}