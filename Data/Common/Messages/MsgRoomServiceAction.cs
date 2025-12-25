using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomServiceAction
    {
        [Key(0)]
        public bool? allowNewRoom;
        [Key(1)]
        public int? saveIntervalS;
    }

    [MessagePackObject]
    public class ResRoomServiceAction
    {

    }
}