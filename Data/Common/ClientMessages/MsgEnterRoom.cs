using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgEnterRoom
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResEnterRoom
    {

    }
}