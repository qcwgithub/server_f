using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgSocketClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }

    [MessagePackObject]
    public class ResSocketClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }
}