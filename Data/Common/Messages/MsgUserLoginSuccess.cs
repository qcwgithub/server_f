using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgUserLoginSuccess
    {
        [Key(0)]
        public MsgUserLogin innerMsg;
        [Key(1)]
        public ResUserLogin innerRes;
    }

    [MessagePackObject]
    public class ResUserLoginSuccess
    {

    }
}