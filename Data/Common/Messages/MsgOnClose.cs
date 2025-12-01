using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgOnClose
    {
        [Key(0)]
        public bool isAcceptor;
        // public bool isServer;
    }    
}