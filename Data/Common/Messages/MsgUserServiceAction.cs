using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgUserServiceAction
    {
        [Key(0)]
        public bool? allowNewUser;
        [Key(1)]
        public int? saveIntervalS;
    }

    [MessagePackObject]
    public class ResUserServiceAction
    {

    }
}